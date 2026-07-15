namespace Hoaii.Domain.Entities;

public enum ProductBadge
{
    None,
    New,
    Sale,
    OutOfStock,
}

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public ProductBadge Badge { get; set; } = ProductBadge.None;
    public bool IsFeatured { get; set; }

    /// <summary>Hidden products stay in past orders but disappear from the storefront.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Manual position within its category; ties break by Id.</summary>
    public int SortOrder { get; set; }

    // SEO — the storefront currently uses the product name as the page title and has no meta
    // description at all.
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<ProductImage> Images { get; set; } = [];
    public ICollection<ProductVariant> Variants { get; set; } = [];
}
