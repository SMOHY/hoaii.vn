namespace Hoaii.Domain.Entities;

/// <summary>A named product line/theme that spans categories — e.g. "Thiên Điểu Lạc Hồng" ships
/// both as a Tết gift set and, separately, as a Trung Thu mooncake box. There was no such concept
/// in the data model before; the mega-menu's "Theo bộ sưu tập" column used to link to sibling
/// occasion categories instead, which doesn't match how the client actually thinks about their
/// product lines. Admin creates/renames/deletes these directly (Areas/Admin/Views/Collections).
///
/// Also has its own public landing page (CollectionController, /bo-suu-tap/{slug}), which reuses
/// Category's page layout wholesale (Views/Category/Index.cshtml) — see CategoryPageViewModel and
/// CollectionHeroSlide. Only the CMS fields that layout actually needs are here; the
/// category-only ones (HeroStyle's Banner variant, CoverImageUrl, ParentLabel/ParentUrl) exist to
/// serve legacy occasion-listing needs collections don't have, so collections always render the
/// Carousel hero style with no parent breadcrumb.</summary>
public class Collection
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public int SortOrder { get; set; }

    public string? Description { get; set; }
    public string? HeroEyebrow { get; set; }
    public string? HeroKicker { get; set; }

    public string? PromoEyebrow { get; set; }
    public string? PromoTitle { get; set; }
    public string? PromoCtaText { get; set; }
    public string? PromoCtaUrl { get; set; }
    public string? PromoImageUrl { get; set; }
    public string? PromoBackground { get; set; }
    public bool PromoWide { get; set; }
}
