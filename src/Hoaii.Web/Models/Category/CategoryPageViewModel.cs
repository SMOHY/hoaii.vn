namespace Hoaii.Web.Models.Category;

public class CategoryPageViewModel
{
    public required string Title { get; init; }
    public required string BreadcrumbLabel { get; init; }

    /// <summary>Price band the shopper picked, or "" for all. See CategoryController.PriceFilters.</summary>
    public string PriceFilter { get; init; } = "";

    /// <summary>True when "còn hàng" is ticked.</summary>
    public bool InStockOnly { get; init; }

    /// <summary>How many filters are active — shown on the Bộ lọc button.</summary>
    public int ActiveFilterCount => (PriceFilter.Length > 0 ? 1 : 0) + (InStockOnly ? 1 : 0);
    public required string Description { get; init; }
    public required IReadOnlyList<ProductCardViewModel> Products { get; init; }
    public int CurrentPage { get; init; } = 1;
    public int TotalPages { get; init; } = 1;
    public required PromoBannerViewModel Promo { get; init; }

    /// <summary>Category slug, so the sort links can rebuild this page's URL.</summary>
    public string Slug { get; init; } = "";

    /// <summary>Active sort key — one of CategoryController.SortOptions.</summary>
    public string Sort { get; init; } = "noi-bat";

    /// <summary>Count across all pages, shown next to the heading.</summary>
    public int TotalProducts { get; init; }

    /// <summary>Slides for the red hero carousel (node 1519:33997) — the category's products.</summary>
    public required IReadOnlyList<HeroSlideViewModel> HeroSlides { get; init; }

    /// <summary>Eyebrow above the hero carousel, e.g. "Quà tết đặc sắc".</summary>
    public required string HeroEyebrow { get; init; }

    /// <summary>Kicker between the pager arrows, e.g. "Bộ quà 6 hộp". Empty hides the line.</summary>
    public string HeroKicker { get; init; } = "";
}

public class HeroSlideViewModel
{
    public required string ImageUrl { get; init; }
    public required string Name { get; init; }
    public required string Slug { get; init; }
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

    /// <summary>Real variant names from the DB. ProductVariant has no colour column, so the
    /// card lists names instead of inventing swatch colours.</summary>
    public IReadOnlyList<string> VariantNames { get; init; } = [];
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
