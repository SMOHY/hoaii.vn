namespace Hoaii.Domain.Entities;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    public int? ProductVariantId { get; set; }

    // Snapshot fields — preserve what the customer actually saw/bought even if the
    // product catalog changes later.
    public required string ProductName { get; set; }
    public string? VariantName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}
