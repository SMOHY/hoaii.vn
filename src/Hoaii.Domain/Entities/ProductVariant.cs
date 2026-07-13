namespace Hoaii.Domain.Entities;

public class ProductVariant
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public required string Name { get; set; } // e.g. "Hộp 4 bánh / màu vàng"
    public decimal PriceModifier { get; set; }
    public string? Sku { get; set; }
    public int StockQuantity { get; set; }
}
