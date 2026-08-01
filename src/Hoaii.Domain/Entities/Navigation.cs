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
