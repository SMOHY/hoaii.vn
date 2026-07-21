namespace Hoaii.Web.Models.Occasion;

/// <summary>
/// One landing page in the "Quà theo dịp" family. Both the umbrella page (node 769:12176) and the
/// per-recipient page (node 778:22062) are the same layout — hero, chooser, N feature sections,
/// campaign — so they share this model and one view. Only the data differs.
/// </summary>
public class OccasionLandingViewModel
{
    public required string Title { get; init; }

    /// <summary>Big display heading in the hero. Not always the same as <see cref="Title"/>.</summary>
    public required string HeroHeading { get; init; }
    public required string HeroSubtitle { get; init; }
    public required string HeroImageUrl { get; init; }
    public string MetaDescription { get; init; } = "";

    /// <summary>Trail after "Trang chủ" — one entry on the umbrella page, two on a child page.</summary>
    public required IReadOnlyList<BreadcrumbItem> Breadcrumb { get; init; }

    /// <summary>The three top-level gift routes; exactly one is marked active.</summary>
    public required IReadOnlyList<OccasionChooserItemViewModel> Chooser { get; init; }

    public required IReadOnlyList<OccasionSectionViewModel> Sections { get; init; }
    public required OccasionCampaignViewModel Campaign { get; init; }
}

public record BreadcrumbItem(string Label, string? Url);

public class OccasionChooserItemViewModel
{
    public required string Title { get; init; }
    public required string Url { get; init; }
    public bool IsActive { get; init; }

    /// <summary>Only the active column shows copy in Figma (node 769:15199); the others ship their
    /// text hidden, so an empty string here is the design, not missing data.</summary>
    public string Description { get; init; } = "";

    /// <summary>Null renders the flat tone Figma uses — the design has no photograph here yet.</summary>
    public string? ImageUrl { get; init; }
}

public class OccasionSectionViewModel
{
    public required string Title { get; init; }
    public required string Description { get; init; }

    /// <summary>Null renders the grey-200 block Figma draws (node 769:15371).</summary>
    public string? CoverImageUrl { get; init; }

    /// <summary>Which side the cover sits on. Figma alternates down the page, so this is the only
    /// thing that differs between the three sections' markup.</summary>
    public bool CoverOnLeft { get; init; }

    public required string ViewAllUrl { get; init; }
    public required IReadOnlyList<CompactProductViewModel> Products { get; init; }
}

/// <summary>The small two-up card used inside a section — not the big grid card from
/// _ProductCard.cshtml: no badge, no swatches, no price comparison (node 769:15309).</summary>
public class CompactProductViewModel
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Slug { get; init; }
    public required decimal Price { get; init; }
    public string? ImageUrl { get; init; }
    public bool IsOutOfStock { get; init; }
}

public class OccasionCampaignViewModel
{
    public required string Eyebrow { get; init; }
    public required string Body { get; init; }
    public required string CtaText { get; init; }
    public required string CtaUrl { get; init; }
    public string? ImageUrl { get; init; }
}
