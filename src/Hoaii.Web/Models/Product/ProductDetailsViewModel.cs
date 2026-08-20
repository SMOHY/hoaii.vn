using Hoaii.Web.Models.Category;

namespace Hoaii.Web.Models.Product;

public class ProductDetailsViewModel
{
    public required int ProductId { get; init; }
    public required string Slug { get; init; }
    public required string Name { get; init; }
    public required decimal Price { get; init; }
    public required string BreadcrumbLabel { get; init; }
    public required string CategorySlug { get; init; }
    public string? MetaDescription { get; init; }

    /// <summary>Badge "Hết hàng" của sản phẩm. Trang chi tiết từng bỏ qua hoàn toàn cờ này, nên
    /// thẻ trên lưới ghi "Hết hàng" mà bấm vào trang vẫn đặt mua được như thường.</summary>
    public required bool IsOutOfStock { get; init; }

    public required IReadOnlyList<string?> GalleryImages { get; init; } // null entries render as placeholder tiles

    /// <summary>Video giới thiệu: tệp trên máy chủ. Ưu tiên hơn link nhúng khi có cả hai.</summary>
    public string? VideoFileUrl { get; init; }

    /// <summary>Video giới thiệu: đường dẫn nhúng YouTube/Vimeo đã đổi dạng sẵn.</summary>
    public string? VideoEmbedUrl { get; init; }

    /// <summary>Ảnh cho ô video trong dải ảnh. Rỗng thì lấy ảnh đầu tiên.</summary>
    public string? VideoPosterUrl { get; init; }

    public bool HasVideo => !string.IsNullOrWhiteSpace(VideoFileUrl) || !string.IsNullOrWhiteSpace(VideoEmbedUrl);

    // No colour axis: Figma ships the colour picker hidden (node 826:20630).
    public required IReadOnlyList<BoxOptionViewModel> BoxOptions { get; init; }

    public required string Ingredients { get; init; }

    public required string StoryTitle { get; init; }
    public required string StoryBody { get; init; }
    public string? StoryImageUrl { get; init; }

    /// <summary>Admin-set replacement below 768px; null keeps the desktop image.</summary>
    public string? StoryImageUrlMobile { get; init; }

    /// <summary>Admin-set focal point ("50% 30%"); null centres as before.</summary>
    public string? StoryImageFocal { get; init; }

    public required string FeatureTitle { get; init; }
    public required string FeatureBody { get; init; }
    public string? FeatureImageUrl { get; init; }

    /// <summary>Admin-set replacement below 768px; null keeps the desktop image.</summary>
    public string? FeatureImageUrlMobile { get; init; }

    /// <summary>Admin-set focal point ("50% 30%"); null centres as before.</summary>
    public string? FeatureImageFocal { get; init; }

    public CollectionSectionViewModel? Collection { get; init; }
    public required IReadOnlyList<ProductCardViewModel> RelatedProducts { get; init; }
}

public class BoxOptionViewModel
{
    public required int Id { get; init; }
    public required string Label { get; init; }
    // Giá bán đầy đủ của biến thể = Product.Price + variant.PriceModifier. Có sẵn ở đây để PDP
    // đổi giá hiển thị khi khách chọn loại hộp, khớp với cách CartService tính (base + delta).
    public required decimal Price { get; init; }
}

public class CollectionSectionViewModel
{
    public required string Eyebrow { get; init; }
    public required string Title { get; init; }
    public required IReadOnlyList<ProductCardViewModel> Items { get; init; }
}
