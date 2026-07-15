using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Models.Layout;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Hoaii.Web.Services;

/// <summary>
/// Supplies the header menus and footer columns to the layout partials from the database,
/// caching the whole set (it is read on every page). The cache is dropped when the admin edits.
/// </summary>
public class NavigationService(HoaiiDbContext db, IMemoryCache cache)
{
    private const string CacheKey = "navigation_all";

    private sealed record Snapshot(
        IReadOnlyList<NavMenuItem> Main,
        IReadOnlyList<NavMenuItem> Sub,
        IReadOnlyList<FooterColumn> Footer);

    private Snapshot Load() =>
        cache.GetOrCreate(CacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

            var links = db.NavLinks.AsNoTracking().OrderBy(l => l.SortOrder).ThenBy(l => l.Id).ToList();
            var columns = db.FooterMenuColumns.AsNoTracking()
                .Include(c => c.Links.OrderBy(l => l.SortOrder))
                .OrderBy(c => c.SortOrder).ThenBy(c => c.Id)
                .ToList();

            return new Snapshot(
                links.Where(l => l.Placement == NavPlacement.Main)
                    .Select(l => new NavMenuItem { Label = l.Label, Url = l.Url, HasDropdown = l.HasDropdown }).ToList(),
                links.Where(l => l.Placement == NavPlacement.Sub)
                    .Select(l => new NavMenuItem { Label = l.Label, Url = l.Url }).ToList(),
                columns.Select(c => new FooterColumn
                {
                    Title = c.Title,
                    Links = c.Links.Select(l => new FooterLink { Label = l.Label, Url = l.Url }).ToList(),
                }).ToList());
        })!;

    public IReadOnlyList<NavMenuItem> MainMenu => Load().Main;
    public IReadOnlyList<NavMenuItem> SubNavLinks => Load().Sub;
    public IReadOnlyList<FooterColumn> FooterColumns => Load().Footer;

    public void Invalidate() => cache.Remove(CacheKey);
}
