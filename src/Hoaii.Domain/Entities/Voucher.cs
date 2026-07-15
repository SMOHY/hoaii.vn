namespace Hoaii.Domain.Entities;

public enum VoucherType
{
    Percentage,   // Value = percent off the subtotal (0–100), optionally capped by MaxDiscountAmount
    FixedAmount,  // Value = VND off the subtotal
    FreeShipping, // waives the shipping fee; Value ignored
}

public class Voucher
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public string Label { get; set; } = "";
    public string Tag { get; set; } = "Voucher";
    public VoucherType Type { get; set; }

    /// <summary>Percent (0–100) for Percentage, VND for FixedAmount, ignored for FreeShipping.</summary>
    public decimal Value { get; set; }

    /// <summary>Cart subtotal must reach this before the code applies.</summary>
    public decimal MinOrderAmount { get; set; }

    /// <summary>Optional ceiling for a percentage discount (VND).</summary>
    public decimal? MaxDiscountAmount { get; set; }

    /// <summary>Optional total redemption cap across all customers.</summary>
    public int? UsageLimit { get; set; }
    public int UsedCount { get; set; }

    public DateTime? StartsAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;

    /// <summary>Whether the code can be used right now for a cart of the given subtotal.</summary>
    public bool IsUsableFor(decimal subtotal, DateTime now) =>
        IsActive
        && (StartsAt is null || StartsAt <= now)
        && (ExpiresAt is null || ExpiresAt >= now)
        && (UsageLimit is null || UsedCount < UsageLimit)
        && subtotal >= MinOrderAmount;

    /// <summary>The VND discount off the subtotal (0 for free-shipping).</summary>
    public decimal DiscountFor(decimal subtotal)
    {
        switch (Type)
        {
            case VoucherType.Percentage:
                var raw = Math.Round(subtotal * Value / 100m, 0);
                return MaxDiscountAmount is { } cap && raw > cap ? cap : raw;
            case VoucherType.FixedAmount:
                return Math.Min(Value, subtotal);
            default:
                return 0m;
        }
    }
}
