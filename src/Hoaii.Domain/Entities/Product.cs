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

    // Product detail page copy. Null falls back to a name-based default so every product still
    // renders a story and a feature block before anyone edits them.
    public string? StoryTitle { get; set; }
    public string? StoryBody { get; set; }
    public string? StoryImageUrl { get; set; }
    public string? FeatureTitle { get; set; }
    public string? FeatureBody { get; set; }
    public string? FeatureImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    /// <summary>Same product line shipped as a different item for another occasion — e.g.
    /// "Thiên Điểu Lạc Hồng" as a Tết box and, separately, as a Trung Thu mooncake box. Nullable:
    /// most products (Trà, Khăn, Rượu, Tượng gốm — the everyday catalog) don't belong to a
    /// cross-occasion line at all.</summary>
    public int? CollectionId { get; set; }
    public Collection? Collection { get; set; }

    public ICollection<ProductImage> Images { get; set; } = [];
    public ICollection<ProductVariant> Variants { get; set; } = [];
}
