namespace Hoaii.Domain.Entities;

// The homepage sections, moved out of HomeController so the shop owner can edit them. Each list
// is ordered by SortOrder; the storefront markup and Figma layout are unchanged — only the source.

public class HomeHeroSlide
{
    public int Id { get; set; }
    public required string ImageUrl { get; set; }
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";
    public string MobileTitle { get; set; } = "";
    public string MobileSubtitle { get; set; } = "";
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class HomeBenefit
{
    public int Id { get; set; }
    public string IconPath { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string MobileLine1 { get; set; } = "";
    public string MobileLine2 { get; set; } = "";
    public int SortOrder { get; set; }
}

public class HomeFeaturedTile
{
    public int Id { get; set; }
    public bool IsCard { get; set; }
    public string? AccentColor { get; set; } // "red" | "teal" | "yellow"
    public string? CollectionLabel { get; set; }
    public string? TitleLine1 { get; set; }
    public string? TitleLine2 { get; set; }
    public string? EditionLabel { get; set; }
    public bool HideOnMobile { get; set; }
    public string? ImageUrl { get; set; }
    public string LinkUrl { get; set; } = "#";
    public int SortOrder { get; set; }
}

public class HomeServiceTab
{
    public int Id { get; set; }
    public required string Key { get; set; }
    public string Label { get; set; } = "";
    public string IconSvg { get; set; } = "";
    public string PanelImageUrl { get; set; } = "";
    public string Caption { get; set; } = "";
    public string CaptionColorHex { get; set; } = "#F2F2F2";
    public string CtaText { get; set; } = "Bắt đầu";
    public string CtaUrl { get; set; } = "#";
    public int SortOrder { get; set; }
}

public class HomeAboutCard
{
    public int Id { get; set; }
    public string Caption { get; set; } = "";
    public bool ImageOnTop { get; set; }
    public string ImageUrl { get; set; } = "";
    public int SortOrder { get; set; }
}

public class HomeCustomerLogo
{
    public int Id { get; set; }
    /// <summary>Basename of the logo asset under /images/customers/, e.g. "truong-thanh".</summary>
    public required string LogoKey { get; set; }
    public int SortOrder { get; set; }
}
