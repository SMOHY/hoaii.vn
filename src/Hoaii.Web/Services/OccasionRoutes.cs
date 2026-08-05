namespace Hoaii.Web.Services;

/// <summary>The 3-way "Quà theo dịp" chooser (Figma node 769:15244): 2 real grouping landing
/// pages plus a link to the partnership page for corporate gifting. Shared between
/// OccasionController (renders the chooser) and MegaMenuViewComponent (the "Quà tặng" column of
/// the mega-menu panel needs the exact same 3 links) so the two can't drift apart — that drift
/// is exactly what caused the mega-menu to show a flat list of 5 unrelated occasion categories
/// instead of these 3 grouped links.</summary>
public static class OccasionRoutes
{
    public static readonly (string Title, string Route, string Thumb)[] ChooserRoutes =
    [
        ("Quà tặng theo dịp", "/qua-theo-dip", "/images/products/tet/ma-dao-thanh-cong.jpg"),
        ("Quà tặng cá nhân", "/qua-tang-ca-nhan", "/images/products/tet/viet-nam-hoa-thi.jpg"),
        // No corporate-gift category exists yet, so this points at the real partnership page
        // rather than inventing a route that would 404. See WF-014.
        ("Quà tặng doanh nghiệp", "/hop-tac", "/images/category/promo-artist.jpg"),
    ];
}
