using Hoaii.Domain.Entities;
using Hoaii.Web.Areas.Admin.Controllers;
using Hoaii.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Services;

/// <summary>Runs once: turns the 8 hard-coded CuratedSlots ("Bán chạy nhất"/etc, defined in
/// MenuController) and the 3 other auto-computed columns ("Theo bộ sưu tập" ×2, "Sản phẩm") into
/// real MegaMenuColumn rows, migrating any existing MegaMenuCuratedItem picks along the way.
/// Before this, only admin-added columns had rename/delete — the built-in 8 were stuck as fixed
/// C# constants, which is the exact inconsistency the client flagged ("sao mấy cái này ko có CRUD
/// cho nó à"). After this, every column in every panel — except "Quà tặng" on "Quà theo dịp",
/// which manages page groupings rather than products/categories and keeps its own dedicated UI —
/// is the same kind of row, editable the same way.</summary>
public static class MegaMenuColumnMigrationSeeder
{
    public static async Task EnsureSeedAsync(HoaiiDbContext db)
    {
        if (await db.MegaMenuColumns.AnyAsync())
        {
            return;
        }

        foreach (var slot in MenuController.CuratedSlots)
        {
            // Figma order per panel: Quà tết/Trung thu = Bán chạy nhất, Phiên bản giới hạn, Theo
            // bộ sưu tập. Quà theo dịp = Quà tặng (not a MegaMenuColumn, rendered separately),
            // Hoài gợi ý, Bán chạy nhất. Sản phẩm chọn lọc = Sản phẩm, Bán chạy nhất, Nổi bật.
            var sortOrder = slot.ColumnKey switch
            {
                "best-sellers" when slot.PanelKey is "qua-tet" or "qua-trung-thu" => 0,
                "limited" => 1,
                "suggested" => 0,
                "best-sellers" => 1, // qua-theo-dip or san-pham-chon-loc
                "featured" => 2,
                _ => 0,
            };
            var column = new MegaMenuColumn { PanelKey = slot.PanelKey, Title = slot.ColumnLabel, Kind = MegaMenuColumnKind.Pick, SortOrder = sortOrder };
            db.MegaMenuColumns.Add(column);
            await db.SaveChangesAsync(); // need column.Id for the items below

            var picks = await db.MegaMenuCuratedItems
                .Where(x => x.PanelKey == slot.PanelKey && x.ColumnKey == slot.ColumnKey)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();
            foreach (var pick in picks)
            {
                db.MegaMenuColumnItems.Add(new MegaMenuColumnItem { MegaMenuColumnId = column.Id, ProductId = pick.ProductId, SortOrder = pick.SortOrder });
            }
        }

        // "Theo bộ sưu tập" (Quà tết / Quà trung thu) used to auto-detect whichever collection
        // the current category's products happened to use. Pinning it to the one real
        // collection that exists today keeps live behaviour the same; admin can repoint it like
        // any other Collection-kind column.
        var firstCollection = await db.Collections.OrderBy(c => c.SortOrder).FirstOrDefaultAsync();
        foreach (var panelKey in new[] { "qua-tet", "qua-trung-thu" })
        {
            db.MegaMenuColumns.Add(new MegaMenuColumn
            {
                PanelKey = panelKey,
                Title = "Theo bộ sưu tập",
                Kind = MegaMenuColumnKind.Collection,
                CollectionId = firstCollection?.Id,
                SortOrder = 2,
            });
        }

        // "Sản phẩm" (Sản phẩm chọn lọc) used to auto-list every ProductType category. Same
        // list, now as admin-picked category links instead of an automatic query. "ruou"
        // (alcohol) stays out — conditional business line held back until the retail licence
        // is in hand, same reason MenuController's LoadDestinationOptionsAsync excludes it too.
        var productTypeCategories = await db.Categories
            .Where(c => c.Type == CategoryType.ProductType && c.Slug != "ruou")
            .OrderBy(c => c.SortOrder).ThenBy(c => c.Id)
            .ToListAsync();
        if (productTypeCategories.Count > 0)
        {
            var col = new MegaMenuColumn { PanelKey = "san-pham-chon-loc", Title = "Sản phẩm", Kind = MegaMenuColumnKind.CategoryLinks, SortOrder = 0 };
            db.MegaMenuColumns.Add(col);
            await db.SaveChangesAsync();
            var order = 0;
            foreach (var cat in productTypeCategories.Take(4))
            {
                db.MegaMenuColumnItems.Add(new MegaMenuColumnItem { MegaMenuColumnId = col.Id, CategoryId = cat.Id, SortOrder = order++ });
            }
        }

        await db.SaveChangesAsync();
    }
}
