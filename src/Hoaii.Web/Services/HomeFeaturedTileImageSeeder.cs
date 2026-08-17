using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Services;

/// <summary>Runs once: moves the "Trung Thu 2026" campaign photos that used to be hardcoded in
/// Views/Home/Sections/_FeaturedProducts.cshtml (keyed by grid position) into the matching
/// HomeFeaturedTile.ImageUrl row, now that the view always renders ImageUrl instead of
/// overriding it. Without this, removing the hardcode would make those 6 tiles fall back to
/// their original seed placeholders (/images/placeholders/featured-*.jpg) the moment this ships
/// — a real image regression on the live homepage. Only touches rows still on that exact
/// placeholder path, so an admin who already picked a real image via /admin/trang-chu before
/// this deployed keeps their choice.</summary>
public static class HomeFeaturedTileImageSeeder
{
    private static readonly (int SortOrder, string PlaceholderUrl, string RealUrl)[] Backfills =
    [
        (1, "/images/placeholders/featured-2.jpg", "/images/home/featured/tinh-hoa-bac-bo-01.webp"),
        (2, "/images/placeholders/featured-3.jpg", "/images/home/featured/tinh-hoa-bac-bo-02.webp"),
        (3, "/images/placeholders/featured-4.jpg", "/images/home/featured/thien-dieu-lux-01.webp"),
        (4, "/images/placeholders/featured-5.jpg", "/images/home/featured/thien-dieu-lux-02.webp"),
        (7, "/images/placeholders/featured-6.jpg", "/images/home/featured/thien-dieu-standard-01.webp"),
        (8, "/images/placeholders/featured-7.jpg", "/images/home/featured/thien-dieu-standard-02.webp"),
    ];

    public static async Task EnsureSeedAsync(HoaiiDbContext db)
    {
        var tiles = await db.HomeFeaturedTiles
            .Where(t => !t.IsCard)
            .ToDictionaryAsync(t => t.SortOrder);

        var changed = false;
        foreach (var (sortOrder, placeholder, real) in Backfills)
        {
            if (tiles.TryGetValue(sortOrder, out var tile) && tile.ImageUrl == placeholder)
            {
                tile.ImageUrl = real;
                changed = true;
            }
        }

        if (changed)
        {
            await db.SaveChangesAsync();
        }
    }
}
