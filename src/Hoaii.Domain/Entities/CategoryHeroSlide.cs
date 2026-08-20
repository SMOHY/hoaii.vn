namespace Hoaii.Domain.Entities;

/// <summary>An admin-curated slide in a category's hero carousel (node 1519:33997), decoupled from
/// the category's products. CategoryController falls back to auto-building slides from the
/// category's own products when a category has none of these — see CategoryController.Index — so
/// existing categories keep rendering until an admin adds a slide here.</summary>
public class CategoryHeroSlide
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public required string ImageUrl { get; set; }

    /// <summary>Optional replacement shown below 768px. Empty means the desktop image is used on
    /// every screen — how every row starts out, so nothing changes until an admin sets one.</summary>
    public string? MobileImageUrl { get; set; }

    /// <summary>Focal point as a CSS object-position pair ("50% 30%"). Empty means centre.</summary>
    public string? ImageFocal { get; set; }
    public string Name { get; set; } = "";
    public string LinkUrl { get; set; } = "#";
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
