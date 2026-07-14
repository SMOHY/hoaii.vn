namespace Hoaii.Web.Models.Home;

public class HomeIndexViewModel
{
    /// <summary>
    /// Hero slides. Figma draws arrows and four dots, but only one hero image was delivered —
    /// with a single slide the controls are not rendered at all rather than sitting there dead.
    /// Add more slides here and the carousel comes to life on its own.
    /// </summary>
    public required IReadOnlyList<HeroSlideViewModel> HeroSlides { get; init; }

    public required IReadOnlyList<BenefitViewModel> Benefits { get; init; }
    public required IReadOnlyList<FeaturedTileViewModel> FeaturedTiles { get; init; }
    public required IReadOnlyList<CustomServiceTabViewModel> CustomServiceTabs { get; init; }
    public required IReadOnlyList<AboutCardViewModel> AboutCards { get; init; }
    public required IReadOnlyList<string> CustomerLogos { get; init; }
    public required IReadOnlyList<BlogPostViewModel> BlogPosts { get; init; }
}

public class HeroSlideViewModel
{
    public required string ImageUrl { get; init; }

    // Figma ships different copy per breakpoint (desktop 1214:38726 / mobile 1062:12563).
    public required string Title { get; init; }
    public required string Subtitle { get; init; }
    public required string MobileTitle { get; init; }
    public required string MobileSubtitle { get; init; }
}

public class BenefitViewModel
{
    public required string IconPath { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string MobileLine1 { get; init; }
    public required string MobileLine2 { get; init; }
}

public class FeaturedTileViewModel
{
    public bool IsCard { get; init; }
    public string? AccentColor { get; init; } // "red" | "teal" | "gold"
    public string? CollectionLabel { get; init; }
    public string? TitleLine1 { get; init; }
    public string? TitleLine2 { get; init; }

    /// <summary>Edition line under the title, e.g. "(Phiên bản thường)". Desktop only; Figma omits it on mobile.</summary>
    public string? EditionLabel { get; init; }

    /// <summary>Figma's mobile grid is 2 columns of 3 rows (6 tiles) against the desktop 3×3 (9 tiles),
    /// so three of the photo tiles drop out below 768px.</summary>
    public bool HideOnMobile { get; init; }

    public string? ImageUrl { get; init; }
    public string LinkUrl { get; init; } = "#";
}

public class CustomServiceTabViewModel
{
    public required string Key { get; init; }
    public required string Label { get; init; }
    public required string IconSvg { get; init; }
    public required string PanelImageUrl { get; init; }
    public required string Caption { get; init; }
    public required string CaptionColorHex { get; init; }
    public string CtaText { get; init; } = "Bắt đầu";
    public string CtaUrl { get; init; } = "#";
}

public class AboutCardViewModel
{
    public required string Caption { get; init; }
    public required bool ImageOnTop { get; init; }
    public required string ImageUrl { get; init; }
}

public class BlogPostViewModel
{
    public required string Category { get; init; }
    public required string Title { get; init; }
    public string? Excerpt { get; init; }
    public required string Url { get; init; }
    public required string ImageUrl { get; init; }
    public bool IsFeatured { get; init; }
}
