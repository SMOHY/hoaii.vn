namespace Hoaii.Domain.Entities;

/// <summary>Groups sibling Occasion categories under one of the "Quà theo dịp" chooser landing
/// pages (OccasionController.Pages, matched by <see cref="Route"/>) — e.g. "Quà tặng theo dịp"
/// groups Ngày lễ tình yêu / Ngày quốc tế phụ nữ / Quà giáng sinh. Before this existed, that
/// grouping was hard-coded in OccasionController and the mega-menu had no way to know it, which
/// is what caused the mega-menu's "Quà tặng" column to show a flat, ungrouped list. Admin can
/// rename a group and move categories between the existing groups; adding a brand-new group
/// still needs a matching landing page built in code, so this doesn't (yet) let admin invent an
/// entirely new branch of the chooser.</summary>
public class CategoryGroup
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Route { get; set; }
    public int SortOrder { get; set; }
    public ICollection<Category> Categories { get; set; } = [];
}
