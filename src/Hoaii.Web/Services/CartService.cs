using System.Text.Json;
using Hoaii.Domain.Entities;
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

    /// <summary>Validates the code against the DB and the current cart subtotal, storing it on
    /// the session when it applies. Returns false for unknown / expired / below-minimum codes.</summary>
    public async Task<bool> ApplyVoucherAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        var trimmed = code.Trim();
        var voucher = await db.Vouchers.FirstOrDefaultAsync(v => v.Code == trimmed);
        if (voucher is null)
        {
            return false;
        }

        var subtotal = await GetSubtotalAsync();
        if (!voucher.IsUsableFor(subtotal, DateTime.UtcNow))
        {
            return false;
        }

        Session.SetString(VoucherSessionKey, voucher.Code);
        return true;
    }

    public void RemoveVoucher() => Session.Remove(VoucherSessionKey);

    private string? GetAppliedVoucherCode() => Session.GetString(VoucherSessionKey);

    private async Task<decimal> GetSubtotalAsync()
    {
        var lines = GetLines();
        if (lines.Count == 0) return 0m;

        var productIds = lines.Select(l => l.ProductId).Distinct().ToList();
        var products = await db.Products
            .Where(p => productIds.Contains(p.Id) && p.IsActive)
            .Include(p => p.Variants)
            .ToDictionaryAsync(p => p.Id);

        var subtotal = 0m;
        foreach (var line in lines)
        {
            if (!products.TryGetValue(line.ProductId, out var product)) continue;
            var variant = line.VariantId is int vid ? product.Variants.FirstOrDefault(v => v.Id == vid) : null;
            subtotal += (product.Price + (variant?.PriceModifier ?? 0)) * line.Quantity;
        }
        return subtotal;
    }

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

        var productIds = lines.Select(l => l.ProductId).Distinct().ToList();
        var products = await db.Products
            .Where(p => productIds.Contains(p.Id) && p.IsActive)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .ToDictionaryAsync(p => p.Id);

        var items = new List<CartItemViewModel>();
        foreach (var line in lines)
        {
            if (!products.TryGetValue(line.ProductId, out var product))
            {
                // Removed, or hidden by the shop since it was added — either way it drops out of
                // the cart instead of staying buyable.
                continue;
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
        var now = DateTime.UtcNow;

        // Only vouchers that are live and reach the cart's minimum are offered / kept applied.
        var usableVouchers = (await db.Vouchers.Where(v => v.IsActive).ToListAsync())
            .Where(v => v.IsUsableFor(subtotal, now))
            .ToList();

        var appliedVoucherCode = GetAppliedVoucherCode();
        var appliedVoucher = appliedVoucherCode is not null
            ? usableVouchers.FirstOrDefault(v => v.Code == appliedVoucherCode)
            : null;

        var discount = appliedVoucher?.DiscountFor(subtotal) ?? 0m;

        return new CartViewModel
        {
            Items = items,
            AddOnSuggestions = await GetAddOnSuggestionsAsync(categoryIds, productIds),
            Subtotal = subtotal,
            Discount = discount,
            AppliedVoucherCode = appliedVoucher?.Code,
            AppliedVoucherLabel = appliedVoucher?.Label,
            FreeShipping = appliedVoucher?.Type == VoucherType.FreeShipping,
            AvailableVouchers = usableVouchers
                .Select(v => new VoucherOption(v.Code, v.Label, v.Tag))
                .ToList(),
        };
    }

    public async Task<int> GetItemCountAsync()
    {
        return GetLines().Sum(l => l.Quantity);
    }

    private async Task<IReadOnlyList<CartAddOnViewModel>> GetAddOnSuggestionsAsync(
        List<int>? categoryIds, List<int>? excludeProductIds = null)
    {
        // Never suggest something the shop has hidden.
        var query = db.Products.Where(p => p.IsActive)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .AsQueryable();

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
            Variants = p.Variants
                .OrderBy(v => v.Id)
                .Select(v => new CartAddOnVariant(v.Id, v.Name, p.Price + v.PriceModifier))
                .ToList(),
        }).ToList();
    }
}
