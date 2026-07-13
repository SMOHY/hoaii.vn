namespace Hoaii.Web.Models.Category;

public class CategoryPageViewModel
{
    public required string Title { get; init; }
    public required string BreadcrumbLabel { get; init; }
    public string? HeroImageUrl { get; init; }
    public required string Description { get; init; }
    public required IReadOnlyList<ProductCardViewModel> Products { get; init; }
    public int CurrentPage { get; init; } = 1;
    public int TotalPages { get; init; } = 1;
    public required PromoBannerViewModel Promo { get; init; }
}

public class ProductCardViewModel
{
    public required int Id { get; init; }
    public required string Slug { get; init; }
    public string? ImageUrl { get; init; }
    public required string Name { get; init; }
    public required decimal Price { get; init; }
    public decimal? CompareAtPrice { get; init; }
    public required string BadgeLabel { get; init; } // "" | "Hàng mới" | "-20%" | "Hết hàng"
    public required string BadgeVariant { get; init; } // "new" | "sale" | "out-of-stock" | ""
    public int VariantCount { get; init; }
    public string CardVariant { get; init; } = "grid"; // "grid" | "collection" | "related"
}

public class PromoBannerViewModel
{
    public required string Eyebrow { get; init; }
    public required string Title { get; init; }
    public required string CtaText { get; init; }
    public required string CtaUrl { get; init; }
    public string? ImageUrl { get; init; }
}
