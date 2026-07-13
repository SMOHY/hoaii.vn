namespace Hoaii.Web.Models.Layout;

public class MegaMenuLinkViewModel
{
    public required string Label { get; init; }
    public required string Url { get; init; }
}

public class MegaMenuColumnViewModel
{
    public required string Title { get; init; }
    public required IReadOnlyList<MegaMenuLinkViewModel> Links { get; init; }
}

public class MegaMenuPanelViewModel
{
    public required string CategoryKey { get; init; } // matches the nav item's data-menu-key
    public required IReadOnlyList<MegaMenuColumnViewModel> Columns { get; init; }
    public required string SeeAllUrl { get; init; }
}
