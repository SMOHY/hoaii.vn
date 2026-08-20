namespace Hoaii.Domain.Entities;

/// <summary>An admin-curated slide in a collection's hero carousel — same idea as
/// CategoryHeroSlide, just scoped to Collection. CollectionController falls back to auto-building
/// slides from the collection's own products when a collection has none of these.</summary>
public class CollectionHeroSlide
{
    public int Id { get; set; }
    public int CollectionId { get; set; }
    public Collection? Collection { get; set; }
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
