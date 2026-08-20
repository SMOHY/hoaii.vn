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

    /// <summary>Optional replacement shown below 768px. Empty means the desktop image is used on
    /// every screen — how every row starts out, so nothing changes until an admin sets one.</summary>
    public string? StoryImageUrlMobile { get; set; }

    /// <summary>Focal point as a CSS object-position pair ("50% 30%"). Empty means centre.</summary>
    public string? StoryImageFocal { get; set; }
    /// <summary>Video giới thiệu/unbox. Hai cách điền, ưu tiên tệp tải lên nếu có cả hai:
    /// <see cref="VideoFileUrl"/> là tệp nằm trên máy chủ, <see cref="VideoEmbedUrl"/> là link
    /// YouTube/Vimeo. Cả hai rỗng thì trang sản phẩm không có gì thay đổi so với trước.</summary>
    public string? VideoFileUrl { get; set; }

    /// <summary>Link YouTube hoặc Vimeo do admin dán vào, ở dạng người dùng copy từ thanh địa chỉ.
    /// Việc đổi sang dạng nhúng được làm lúc hiển thị, để admin không phải hiểu "embed" là gì.</summary>
    public string? VideoEmbedUrl { get; set; }

    /// <summary>Ảnh đại diện cho ô video trong dải ảnh sản phẩm. Rỗng thì dùng ảnh đầu tiên.</summary>
    public string? VideoPosterUrl { get; set; }

    public string? FeatureTitle { get; set; }
    public string? FeatureBody { get; set; }
    public string? FeatureImageUrl { get; set; }

    /// <summary>Optional replacement shown below 768px. Empty means the desktop image is used on
    /// every screen — how every row starts out, so nothing changes until an admin sets one.</summary>
    public string? FeatureImageUrlMobile { get; set; }

    /// <summary>Focal point as a CSS object-position pair ("50% 30%"). Empty means centre.</summary>
    public string? FeatureImageFocal { get; set; }

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
