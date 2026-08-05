namespace Hoaii.Domain.Entities;

/// <summary>A named product line/theme that spans categories — e.g. "Thiên Điểu Lạc Hồng" ships
/// both as a Tết gift set and, separately, as a Trung Thu mooncake box. There was no such concept
/// in the data model before; the mega-menu's "Theo bộ sưu tập" column used to link to sibling
/// occasion categories instead, which doesn't match how the client actually thinks about their
/// product lines. Admin creates/renames/deletes these directly (Areas/Admin/Views/Collections).</summary>
public class Collection
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int SortOrder { get; set; }
}
