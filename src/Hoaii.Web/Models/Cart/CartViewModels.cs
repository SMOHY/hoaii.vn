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
}

public class CartViewModel
{
    public required IReadOnlyList<CartItemViewModel> Items { get; init; }
    public required IReadOnlyList<CartAddOnViewModel> AddOnSuggestions { get; init; }
    public required decimal Subtotal { get; init; }
    public required decimal Discount { get; init; }
    public string? AppliedVoucherCode { get; init; }
    public string? AppliedVoucherLabel { get; init; }

    /// <summary>Set by a free-shipping voucher; the checkout waives the shipping fee.</summary>
    public bool FreeShipping { get; init; }

    /// <summary>Codes shown in the voucher modal (only those usable for the current cart).</summary>
    public IReadOnlyList<VoucherOption> AvailableVouchers { get; init; } = [];

    public decimal Total => Subtotal - Discount;
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
