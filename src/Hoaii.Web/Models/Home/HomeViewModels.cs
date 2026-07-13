namespace Hoaii.Web.Models.Home;

public class HomeIndexViewModel
{
    public required IReadOnlyList<BenefitViewModel> Benefits { get; init; }
    public required IReadOnlyList<FeaturedTileViewModel> FeaturedTiles { get; init; }
    public required IReadOnlyList<CustomServiceTabViewModel> CustomServiceTabs { get; init; }
    public required IReadOnlyList<AboutCardViewModel> AboutCards { get; init; }
    public required IReadOnlyList<string> CustomerLogos { get; init; }
    public required IReadOnlyList<BlogPostViewModel> BlogPosts { get; init; }
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
    public string? AccentColor { get; init; } // "red" | "gold"
    public string? CollectionLabel { get; init; }
    public string? TitleLine1 { get; init; }
    public string? TitleLine2 { get; init; }
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
