using Hoaii.Web.Models.Category;

namespace Hoaii.Web.Models.Product;

public class ProductDetailsViewModel
{
    public required int ProductId { get; init; }
    public required string Slug { get; init; }
    public required string Name { get; init; }
    public required decimal Price { get; init; }
    public required string BreadcrumbLabel { get; init; }
    public string? MetaDescription { get; init; }

    public required IReadOnlyList<string?> GalleryImages { get; init; } // null entries render as placeholder tiles

    // No colour axis: Figma ships the colour picker hidden (node 826:20630).
    public required IReadOnlyList<BoxOptionViewModel> BoxOptions { get; init; }

    public required string Ingredients { get; init; }

    public required string StoryTitle { get; init; }
    public required string StoryBody { get; init; }
    public string? StoryImageUrl { get; init; }

    public required string FeatureTitle { get; init; }
    public required string FeatureBody { get; init; }
    public string? FeatureImageUrl { get; init; }

    public CollectionSectionViewModel? Collection { get; init; }
    public required IReadOnlyList<ProductCardViewModel> RelatedProducts { get; init; }
}

public class BoxOptionViewModel
{
    public required int Id { get; init; }
    public required string Label { get; init; }
}

public class CollectionSectionViewModel
{
    public required string Eyebrow { get; init; }
    public required string Title { get; init; }
    public required IReadOnlyList<ProductCardViewModel> Items { get; init; }
}
