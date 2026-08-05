namespace Hoaii.Domain.Entities;

public enum NavPlacement
{
    Main, // top menu with dropdowns
    Sub,  // secondary bar (Về chúng tôi / Liên hệ / …)
}

/// <summary>A link in the header — either the main menu or the sub bar, ordered by SortOrder.
/// A Main link with HasDropdown can own Children: the submenu links shown in its dropdown panel.</summary>
public class NavLink
{
    public int Id { get; set; }
    public NavPlacement Placement { get; set; }
    public required string Label { get; set; }
    public required string Url { get; set; }
    public bool HasDropdown { get; set; }
    public int SortOrder { get; set; }
    public int? ParentId { get; set; }
    public NavLink? Parent { get; set; }
    public List<NavLink> Children { get; set; } = [];
}

public class FooterMenuColumn
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public int SortOrder { get; set; }
    public List<FooterMenuLink> Links { get; set; } = [];
}

public class FooterMenuLink
{
    public int Id { get; set; }
    public int FooterMenuColumnId { get; set; }
    public required string Label { get; set; }
    public required string Url { get; set; }
    public int SortOrder { get; set; }
    public FooterMenuColumn? Column { get; set; }
}

/// <summary>One admin-picked product inside one column of one of the 4 built-in mega-menu
/// panels (see MegaMenuViewComponent's PanelKey/ColumnKey usage). Replaces the old
/// "auto-pick by IsFeatured/Badge/age" logic for these columns with manual curation — the
/// client doesn't want "bán chạy nhất"/"nổi bật" tied to real sales figures, they want to
/// choose. PanelKey/ColumnKey are plain strings rather than an FK/enum because the 4 panels
/// and their columns are hand-built in code (Figma layout), not database rows.</summary>
public class MegaMenuCuratedItem
{
    public int Id { get; set; }
    public required string PanelKey { get; set; }
    public required string ColumnKey { get; set; }
    public int SortOrder { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
}
