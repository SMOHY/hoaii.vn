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

    /// <summary>Display name of the menu, used to label the panel's close button ("Đóng menu Quà tết").</summary>
    public required string Title { get; init; }

    /// <summary>Product shot filling the right-hand half of the panel (node 923:17080).</summary>
    public string? ImageUrl { get; init; }
}
