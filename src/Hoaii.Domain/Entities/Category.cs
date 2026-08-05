namespace Hoaii.Domain.Entities;

public enum CategoryType
{
    ProductType, // Trà, Khăn, Tượng gốm, Rượu
    Occasion,    // Quà tết, Quà trung thu, Quà theo dịp, ...
}

/// <summary>How a category page opens. Figma draws two: the coverflow of products used by Quà tết
/// and Quà trung thu, and a plain wide banner with the name across it used by the other eight
/// listings (e.g. node 1269:39703).</summary>
public enum CategoryHeroStyle
{
    /// <summary>Default so existing categories keep the hero they already render.</summary>
    Carousel = 0,
    Banner = 1,
}

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public CategoryType Type { get; set; }
    public int SortOrder { get; set; }

    // CMS copy for the category landing page. Null falls back to a sensible default built from
    // the name, so existing categories keep rendering before anyone edits them.
    public string? Description { get; set; }
    public string? HeroEyebrow { get; set; }

    /// <summary>Line under the hero carousel describing what a set contains — "Bộ quà 6 hộp" for
    /// Quà tết, "Bộ quà 4 hộp bánh" for Quà trung thu. It really is per-category, so it cannot
    /// live in the view.</summary>
    public string? HeroKicker { get; set; }
    public string? PromoEyebrow { get; set; }
    public string? PromoTitle { get; set; }
    public string? PromoCtaText { get; set; }
    public string? PromoCtaUrl { get; set; }
    public string? PromoImageUrl { get; set; }

    /// <summary>Nền của dải campaign. Figma đổi màu theo từng danh mục — Quà tết #AA8656,
    /// Quà trung thu #AF2234, các trang còn lại #E5D9CB — nên đây là dữ liệu, không phải một
    /// màu cố định trong CSS. Để trống thì dùng mặc định trong stylesheet.</summary>
    public string? PromoBackground { get; set; }

    /// <summary>Figma vẽ dải campaign theo hai bố cục: bản hẹp (cột chữ và ảnh cùng 760, lề trái
    /// 240 lề phải 80) dùng cho Quà tết và Quà trung thu; bản rộng (ảnh 840, chữ thụt vào 240
    /// trong khung 840, lề đều 80) dùng cho tất cả các trang còn lại.</summary>
    public bool PromoWide { get; set; }

    /// <summary>Wide cover shown beside the category's block on the "Quà theo dịp" landing pages
    /// (node 769:15371). Figma has no photograph placed there yet, so null renders the flat tone
    /// the design draws instead of a broken image.</summary>
    public string? CoverImageUrl { get; set; }

    public CategoryHeroStyle HeroStyle { get; set; }

    /// <summary>Wide image behind the banner hero (node 1269:39703). Figma leaves every one of the
    /// eight banners empty, so null renders the flat block the design draws.</summary>
    public string? BannerImageUrl { get; set; }

    /// <summary>Middle breadcrumb crumb — Figma shows three levels on the occasion listings
    /// (node 1269:39709). Null means the category hangs off the home page directly. Stored as a
    /// label/URL pair rather than a category id because the parent is a landing page
    /// ("Quà theo dịp"), which is not a Category row.</summary>
    public string? ParentLabel { get; set; }
    public string? ParentUrl { get; set; }

    /// <summary>Which "Quà theo dịp" chooser landing page's Sections list this Occasion category
    /// shows up in (see CategoryGroup, OccasionController). Independent of ParentLabel/ParentUrl
    /// above — those are just breadcrumb text, this is what actually drives the landing page and
    /// the mega-menu's "Quà tặng" column.</summary>
    public int? GroupId { get; set; }
    public CategoryGroup? Group { get; set; }

    public ICollection<Product> Products { get; set; } = [];
}
