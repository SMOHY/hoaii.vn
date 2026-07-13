using System.Text.Json;
using Hoaii.Infrastructure;
using Hoaii.Web.Models.Cart;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Services;

/// <summary>
/// Guest cart stored in the ASP.NET Core session (no account/persistence layer yet).
/// Cart contents are just (productId, variantId, quantity) lines; product/price data
/// is always re-hydrated live from the DB so prices/availability stay current.
/// </summary>
public class CartService(IHttpContextAccessor httpContextAccessor, HoaiiDbContext db)
{
    private const string SessionKey = "cart_v1";
    private const string VoucherSessionKey = "cart_voucher_v1";

    // Demo voucher catalog — see design-specs/checkout-and-modals.md ("Voucher Modal").
    // Replace with a real Voucher table/admin UI when promotions need to be managed dynamically.
    public static readonly IReadOnlyList<VoucherDefinition> AvailableVouchers =
    [
        new VoucherDefinition("FREESHIP", "Miễn phí vận chuyển", "Ưu đãi", 0m, IsPercentage: false),
        new VoucherDefinition("GIAM20", "Giảm giá 20%", "Voucher", 0.20m, IsPercentage: true),
    ];

    private ISession Session => httpContextAccessor.HttpContext!.Session;

    private List<CartLine> GetLines()
    {
        var json = Session.GetString(SessionKey);
        if (string.IsNullOrEmpty(json))
        {
            return [];
        }

        return JsonSerializer.Deserialize<List<CartLine>>(json) ?? [];
    }

    private void SaveLines(List<CartLine> lines)
    {
        Session.SetString(SessionKey, JsonSerializer.Serialize(lines));
    }

    public void AddItem(int productId, int? variantId, int quantity)
    {
        if (quantity <= 0)
        {
            return;
        }

        var lines = GetLines();
        var existing = lines.FirstOrDefault(l => l.ProductId == productId && l.VariantId == variantId);
        if (existing is not null)
        {
            lines.Remove(existing);
            lines.Add(existing with { Quantity = existing.Quantity + quantity });
        }
        else
        {
            lines.Add(new CartLine(productId, variantId, quantity));
        }

        SaveLines(lines);
    }

    public void UpdateQuantity(int productId, int? variantId, int quantity)
    {
        var lines = GetLines();
        var existing = lines.FirstOrDefault(l => l.ProductId == productId && l.VariantId == variantId);
        if (existing is null)
        {
            return;
        }

        lines.Remove(existing);
        if (quantity > 0)
        {
            lines.Add(existing with { Quantity = quantity });
        }

        SaveLines(lines);
    }

    public void RemoveItem(int productId, int? variantId)
    {
        var lines = GetLines();
        lines.RemoveAll(l => l.ProductId == productId && l.VariantId == variantId);
        SaveLines(lines);
    }

    public void Clear()
    {
        SaveLines([]);
        RemoveVoucher();
    }

    public bool ApplyVoucher(string code)
    {
        var voucher = AvailableVouchers.FirstOrDefault(v => v.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        if (voucher is null)
        {
            return false;
        }

        Session.SetString(VoucherSessionKey, voucher.Code);
        return true;
    }

    public void RemoveVoucher() => Session.Remove(VoucherSessionKey);

    private string? GetAppliedVoucherCode() => Session.GetString(VoucherSessionKey);

    public async Task<CartViewModel> GetCartAsync()
    {
        var lines = GetLines();

        if (lines.Count == 0)
        {
            return new CartViewModel
            {
                Items = [],
                AddOnSuggestions = await GetAddOnSuggestionsAsync(null),
                Subtotal = 0,
                Discount = 0,
            };
        }

        var appliedVoucherCode = GetAppliedVoucherCode();
        var appliedVoucher = appliedVoucherCode is not null
            ? AvailableVouchers.FirstOrDefault(v => v.Code == appliedVoucherCode)
            : null;

        var productIds = lines.Select(l => l.ProductId).Distinct().ToList();
        var products = await db.Products
            .Where(p => productIds.Contains(p.Id))
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .ToDictionaryAsync(p => p.Id);

        var items = new List<CartItemViewModel>();
        foreach (var line in lines)
        {
            if (!products.TryGetValue(line.ProductId, out var product))
            {
                continue; // product removed/discontinued since it was added to the cart
            }

            var variant = line.VariantId is int variantId
                ? product.Variants.FirstOrDefault(v => v.Id == variantId)
                : null;

            items.Add(new CartItemViewModel
            {
                ProductId = product.Id,
                VariantId = variant?.Id,
                Slug = product.Slug,
                ThumbnailUrl = product.Images.OrderBy(i => i.SortOrder).FirstOrDefault()?.Url,
                Name = product.Name,
                VariantLabel = variant?.Name,
                UnitPrice = product.Price + (variant?.PriceModifier ?? 0),
                Quantity = line.Quantity,
            });
        }

        var categoryIds = products.Values.Select(p => p.CategoryId).Distinct().ToList();
        var subtotal = items.Sum(i => i.LineTotal);
        var discount = appliedVoucher is null ? 0m
            : appliedVoucher.IsPercentage ? Math.Round(subtotal * appliedVoucher.Value, 0)
            : appliedVoucher.Value;

        return new CartViewModel
        {
            Items = items,
            AddOnSuggestions = await GetAddOnSuggestionsAsync(categoryIds, productIds),
            Subtotal = subtotal,
            Discount = discount,
            AppliedVoucherCode = appliedVoucher?.Code,
            AppliedVoucherLabel = appliedVoucher?.Label,
        };
    }

    public async Task<int> GetItemCountAsync()
    {
        return GetLines().Sum(l => l.Quantity);
    }

    private async Task<IReadOnlyList<CartAddOnViewModel>> GetAddOnSuggestionsAsync(
        List<int>? categoryIds, List<int>? excludeProductIds = null)
    {
        var query = db.Products.Include(p => p.Images).AsQueryable();

        if (categoryIds is { Count: > 0 })
        {
            query = query.Where(p => categoryIds.Contains(p.CategoryId));
        }

        if (excludeProductIds is { Count: > 0 })
        {
            query = query.Where(p => !excludeProductIds.Contains(p.Id));
        }

        var products = await query.Take(3).ToListAsync();

        return products.Select(p => new CartAddOnViewModel
        {
            ProductId = p.Id,
            Slug = p.Slug,
            ThumbnailUrl = p.Images.OrderBy(i => i.SortOrder).FirstOrDefault()?.Url,
            Name = p.Name,
            Price = p.Price,
        }).ToList();
    }
}
