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
    public string Name { get; set; } = "";
    public string LinkUrl { get; set; } = "#";
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
