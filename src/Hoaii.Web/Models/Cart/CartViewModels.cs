namespace Hoaii.Web.Models.Cart;

public record CartLine(int ProductId, int? VariantId, int Quantity);

public class CartItemViewModel
{
    public required int ProductId { get; init; }
    public int? VariantId { get; init; }
    public required string Slug { get; init; }
    public string? ThumbnailUrl { get; init; }
    public required string Name { get; init; }
    public string? VariantLabel { get; init; }
    public required decimal UnitPrice { get; init; }
    public required int Quantity { get; init; }
    public decimal LineTotal => UnitPrice * Quantity;
}

public class CartAddOnViewModel
{
    public required int ProductId { get; init; }
    public required string Slug { get; init; }
    public string? ThumbnailUrl { get; init; }
    public required string Name { get; init; }
    public required decimal Price { get; init; }

    /// <summary>Real variant names. Figma draws separate "Size" and "Color" fields in the picker
    /// (node 970:20686), but ProductVariant stores one name and no colour column, so the sheet
    /// offers the names that actually exist rather than two invented dropdowns.</summary>
    public IReadOnlyList<CartAddOnVariant> Variants { get; init; } = [];

    /// <summary>With one option or none there is nothing to choose, so "Thêm" adds straight away.</summary>
    public bool NeedsVariantChoice => Variants.Count > 1;
}

public record CartAddOnVariant(int Id, string Name, decimal Price);

public class CartViewModel
{
    public required IReadOnlyList<CartItemViewModel> Items { get; init; }
    public required IReadOnlyList<CartAddOnViewModel> AddOnSuggestions { get; init; }
    public required decimal Subtotal { get; init; }
    public required decimal Discount { get; init; }

    /// <summary>VAT amount in VND — CartService computes this as VatRatePercent% of
    /// (Subtotal - Discount), never touching shipping (that's added later, at checkout).</summary>
    public required decimal Vat { get; init; }

    /// <summary>The configured rate itself (e.g. 8), just for the "Thuế VAT (8%)" label text.</summary>
    public decimal VatRatePercent { get; init; }
    public string? AppliedVoucherCode { get; init; }
    public string? AppliedVoucherLabel { get; init; }

    /// <summary>Set by a free-shipping voucher; the checkout waives the shipping fee.</summary>
    public bool FreeShipping { get; init; }

    /// <summary>Codes shown in the voucher modal (only those usable for the current cart).</summary>
    public IReadOnlyList<VoucherOption> AvailableVouchers { get; init; } = [];

    public decimal Total => Subtotal - Discount + Vat;
    public int ItemCount => Items.Sum(i => i.Quantity);
}

public record VoucherOption(string Code, string Label, string Tag);

public class VoucherViewModel
{
    public required string Code { get; init; }
    public required string Label { get; init; }
    public required string Tag { get; init; }
    public bool IsSelected { get; init; }
}
