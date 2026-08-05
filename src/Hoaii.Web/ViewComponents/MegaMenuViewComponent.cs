using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Models.Layout;
using Hoaii.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.ViewComponents;

/// <summary>
/// Renders the 4 hover-triggered mega-menu dropdown panels (Quà tết / Quà trung thu /
/// Quà theo dịp / Sản phẩm chọn lọc) seen on the desktop Nav in Figma (node 923:17080).
/// Column headings match the Figma copy exactly; the links underneath are real
/// product/category data rather than the repeated placeholder text Figma used.
/// </summary>
public class MegaMenuViewComponent(HoaiiDbContext db) : ViewComponent
{
    private static readonly string[] OccasionSlugs = ["qua-tet", "qua-trung-thu", "qua-tang-theo-dip"];

    /// <summary>_Nav.cshtml lấy khoá dropdown bằng đoạn cuối của URL trong NavLinks. Mục "Quà theo
    /// dịp" trỏ tới trang landing <c>/qua-theo-dip</c> chứ không phải danh mục
    /// <c>/danh-muc/qua-tang-theo-dip</c>, nên khoá panel phải theo URL đó — nếu để nguyên slug
    /// danh mục thì trigger và panel không khớp nhau và dropdown không mở được.</summary>
    private static string PanelKey(string slug) => slug == "qua-tang-theo-dip" ? "qua-theo-dip" : slug;

    /// <summary>Cùng lý do: "Xem tất cả" của panel này phải về trang landing.</summary>
    private static string SeeAll(string slug) => slug == "qua-tang-theo-dip" ? "/qua-theo-dip" : $"/danh-muc/{slug}";

    /// <summary>Every column of every built-in panel is a MegaMenuColumn row — "Bán chạy nhất",
    /// "Theo bộ sưu tập", "Sản phẩm", and anything an admin added past those, all the same way
    /// (Areas/Admin/Views/Menu/Index.cshtml → "+ Thêm cột"). The one exception is "Quà tặng" on
    /// "Quà theo dịp", which points at page groupings (see CategoryGroup) rather than
    /// products/categories, so it keeps its own dedicated management UI instead of fitting this
    /// generic Pick/Collection/CategoryLinks shape.</summary>
    private async Task<List<MegaMenuColumnViewModel>> ResolveCustomColumnsAsync(string panelKey)
    {
        var customColumns = await db.MegaMenuColumns
            .Where(c => c.PanelKey == panelKey)
            .Include(c => c.Items.OrderBy(i => i.SortOrder))
            .OrderBy(c => c.SortOrder)
            .ToListAsync();
        if (customColumns.Count == 0)
        {
            return [];
        }

        var result = new List<MegaMenuColumnViewModel>();
        foreach (var col in customColumns)
        {
            List<MegaMenuLinkViewModel> links;
            if (col.Kind == MegaMenuColumnKind.Pick)
            {
                var productIds = col.Items.Where(i => i.ProductId != null).Select(i => i.ProductId!.Value).ToList();
                var products = await db.Products.Where(p => productIds.Contains(p.Id) && p.IsActive).ToDictionaryAsync(p => p.Id);
                links = productIds.Where(products.ContainsKey).Take(4)
                    .Select(id => new MegaMenuLinkViewModel { Label = products[id].Name, Url = $"/san-pham/{products[id].Slug}" })
                    .ToList();
            }
            else if (col.Kind == MegaMenuColumnKind.Collection)
            {
                links = col.CollectionId is null
                    ? []
                    : await db.Products
                        .Where(p => p.CollectionId == col.CollectionId && p.IsActive)
                        .OrderBy(p => p.Id).Take(4)
                        .Select(p => new MegaMenuLinkViewModel { Label = p.Name, Url = $"/san-pham/{p.Slug}" })
                        .ToListAsync();
            }
            else
            {
                // Category links have no thumbnail (just a text row), so they tolerate a taller
                // column better than the 4-pick product/collection kinds — e.g. "Quà theo dịp"
                // lists all 5 occasion categories, not just the first 4.
                var categoryIds = col.Items.Where(i => i.CategoryId != null).Select(i => i.CategoryId!.Value).ToList();
                var categories = await db.Categories.Where(c => categoryIds.Contains(c.Id)).ToDictionaryAsync(c => c.Id);
                links = categoryIds.Where(categories.ContainsKey).Take(6)
                    .Select(id => new MegaMenuLinkViewModel { Label = categories[id].Name, Url = $"/danh-muc/{categories[id].Slug}" })
                    .ToList();
            }
            result.Add(new MegaMenuColumnViewModel { Title = col.Title, Links = links });
        }
        return result;
    }

    /// <param name="view">
    /// "Default" renders the desktop hover panels. "Drawer" renders the same links as an
    /// accordion inside the mobile drawer — the panels are hover-only, so on a phone none of
    /// these sub-category links were reachable at all.
    /// </param>
    public async Task<IViewComponentResult> InvokeAsync(string view = "Default")
    {
        var panels = new List<MegaMenuPanelViewModel>();

        var occasionCategories = await db.Categories
            .Where(c => OccasionSlugs.Contains(c.Slug))
            .ToListAsync();

        foreach (var slug in OccasionSlugs)
        {
            var category = occasionCategories.FirstOrDefault(c => c.Slug == slug);
            if (category is null)
            {
                continue;
            }

            var panelKey = PanelKey(slug);
            var columns = new List<MegaMenuColumnViewModel>();

            // "Quà tặng" trỏ tới các trang gộp nhóm (Quà tặng theo dịp/cá nhân/doanh nghiệp), không
            // phải sản phẩm/danh mục, nên không nằm trong MegaMenuColumn — quản lý riêng ở khối
            // "Nhóm danh mục" trong Areas/Admin/Views/Menu/Index.cshtml.
            if (slug == "qua-tang-theo-dip")
            {
                columns.Add(new MegaMenuColumnViewModel
                {
                    Title = "Quà tặng",
                    Links = OccasionRoutes.ChooserRoutes.Select(r => new MegaMenuLinkViewModel { Label = r.Title, Url = r.Route }).ToList(),
                });
            }
            columns.AddRange(await ResolveCustomColumnsAsync(panelKey));

            // Every panel now gets a real photo — the first product image in its category —
            // instead of just "Quà tết"; the other three showed as a flat grey block, which read
            // as a missing image rather than an intentional flat fill.
            var panelImage = await db.ProductImages
                .Where(i => i.Product.CategoryId == category.Id)
                .OrderBy(i => i.ProductId).ThenBy(i => i.SortOrder)
                .Select(i => i.Url)
                .FirstOrDefaultAsync();

            panels.Add(new MegaMenuPanelViewModel
            {
                CategoryKey = panelKey,
                Title = category.Name,
                SeeAllUrl = SeeAll(slug),
                ImageUrl = panelImage,
                Columns = columns,
            });
        }

        // "Sản phẩm chọn lọc" has no literal Category row — it's a cross-category featured view
        // (CategoryController.FeaturedSlug). Its columns (Sản phẩm/Bán chạy nhất/Nổi bật) are
        // MegaMenuColumn rows same as every other panel, seeded once by
        // MegaMenuColumnMigrationSeeder from what used to be hard-coded here.
        var sanPhamColumns = await ResolveCustomColumnsAsync("san-pham-chon-loc");

        // No literal category to pull a photo from here, so use whichever product an admin
        // picked first for this panel's columns (e.g. "Bán chạy nhất") — same "first real pick"
        // rule as every other panel, just sourced from MegaMenuColumnItem instead of Category.
        var sanPhamProductId = await db.MegaMenuColumnItems
            .Where(i => i.Column!.PanelKey == "san-pham-chon-loc" && i.ProductId != null)
            .OrderBy(i => i.Column!.SortOrder).ThenBy(i => i.SortOrder)
            .Select(i => i.ProductId)
            .FirstOrDefaultAsync();
        var sanPhamImage = sanPhamProductId is null
            ? null
            : await db.ProductImages
                .Where(i => i.ProductId == sanPhamProductId)
                .OrderBy(i => i.SortOrder)
                .Select(i => i.Url)
                .FirstOrDefaultAsync();

        panels.Add(new MegaMenuPanelViewModel
        {
            CategoryKey = "san-pham-chon-loc",
            Title = "Sản phẩm chọn lọc",
            SeeAllUrl = "/danh-muc/san-pham-chon-loc",
            ImageUrl = sanPhamImage,
            Columns = sanPhamColumns,
        });

        // Any other Main nav item marked "Dropdown" in Admin → Menu isn't one of the four panels
        // above — those are hand-built from Figma. For everything else the admin's own submenu
        // links (NavLink.Children) are the panel content, one plain column, no photo.
        var builtInKeys = new HashSet<string>(OccasionSlugs.Select(PanelKey)) { "san-pham-chon-loc" };
        var customDropdowns = await db.NavLinks
            .Where(l => l.Placement == NavPlacement.Main && l.ParentId == null && l.HasDropdown)
            .Include(l => l.Children.OrderBy(c => c.SortOrder))
            .ToListAsync();

        foreach (var parent in customDropdowns)
        {
            var key = parent.Url.Split('/').Last();
            if (builtInKeys.Contains(key) || parent.Children.Count == 0)
            {
                continue;
            }

            panels.Add(new MegaMenuPanelViewModel
            {
                CategoryKey = key,
                Title = parent.Label,
                SeeAllUrl = parent.Url,
                ImageUrl = null,
                Columns =
                [
                    new MegaMenuColumnViewModel
                    {
                        Title = parent.Label,
                        Links = parent.Children.Select(c => new MegaMenuLinkViewModel { Label = c.Label, Url = c.Url }).ToList(),
                    },
                ],
            });
        }

        return View(view, panels);
    }
}
