namespace Hoaii.Domain.Entities;

public enum CategoryType
{
    ProductType, // Trà, Khăn, Tượng gốm, Rượu
    Occasion,    // Quà tết, Quà trung thu, Quà theo dịp, ...
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

    public ICollection<Product> Products { get; set; } = [];
}
