using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Models.Layout;
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

            var products = await db.Products
                .Where(p => p.CategoryId == category.Id && p.IsActive)
                .OrderBy(p => p.Id)
                .ToListAsync();

            var bestSellers = products.Take(4)
                .Select(p => new MegaMenuLinkViewModel { Label = p.Name, Url = $"/san-pham/{p.Slug}" })
                .ToList();
            var limited = products.Where(p => p.Badge == ProductBadge.New).Take(4)
                .Select(p => new MegaMenuLinkViewModel { Label = p.Name, Url = $"/san-pham/{p.Slug}" })
                .ToList();
            var otherOccasions = occasionCategories.Where(c => c.Slug != slug)
                .Select(c => new MegaMenuLinkViewModel { Label = c.Name, Url = $"/danh-muc/{c.Slug}" })
                .ToList();
            var suggested = products.Where(p => p.IsFeatured).Take(4)
                .Select(p => new MegaMenuLinkViewModel { Label = p.Name, Url = $"/san-pham/{p.Slug}" })
                .ToList();

            // "Quà tặng theo dịp" gets its own headings and groupings in Figma (node 908:15209):
            // a list of the sibling occasions, then curated picks, then best sellers — where the
            // "Quà tết"/"Quà trung thu" panels (908:15175 / 908:15177) share one layout.
            List<MegaMenuColumnViewModel> columns = slug == "qua-tang-theo-dip"
                ?
                [
                    new MegaMenuColumnViewModel { Title = "Quà tặng", Links = otherOccasions },
                    new MegaMenuColumnViewModel { Title = "Hoài gợi ý", Links = suggested },
                    new MegaMenuColumnViewModel { Title = "Bán chạy nhất", Links = bestSellers },
                ]
                :
                [
                    new MegaMenuColumnViewModel { Title = "Bán chạy nhất", Links = bestSellers },
                    new MegaMenuColumnViewModel { Title = "Phiên bản giới hạn", Links = limited },
                    new MegaMenuColumnViewModel { Title = "Theo bộ sưu tập", Links = otherOccasions },
                ];

            // Only "Quà tết" carries a photo. In Figma the other panels' right-hand half is a
            // flat grey-100 fill with no image placed (nodes 908:15196 / 908:15228 / 908:15260),
            // so filling it with an arbitrary product shot would not match the design.
            var panelImage = slug == "qua-tet"
                ? await db.ProductImages
                    .Where(i => i.Product.CategoryId == category.Id)
                    .OrderBy(i => i.ProductId).ThenBy(i => i.SortOrder)
                    .Select(i => i.Url)
                    .FirstOrDefaultAsync()
                : null;

            panels.Add(new MegaMenuPanelViewModel
            {
                CategoryKey = slug,
                Title = category.Name,
                SeeAllUrl = $"/danh-muc/{slug}",
                ImageUrl = panelImage,
                Columns = columns,
            });
        }

        // "Sản phẩm chọn lọc" has no literal Category row — it's a cross-category
        // featured view (CategoryController.FeaturedSlug), so its columns are sourced differently
        // from the occasion panels above. Headings and order follow Figma node 908:15241:
        // the product types, then best sellers, then the featured picks.
        var productTypes = await db.Categories
            .Where(c => c.Type == CategoryType.ProductType)
            .OrderBy(c => c.Id)
            .Select(c => new MegaMenuLinkViewModel { Label = c.Name, Url = $"/danh-muc/{c.Slug}" })
            .ToListAsync();
        // No sales figures exist yet, so "best sellers" is the oldest live products — the same
        // stand-in the occasion panels use.
        var topSellers = await db.Products.Where(p => p.IsActive).OrderBy(p => p.Id).Take(4)
            .Select(p => new MegaMenuLinkViewModel { Label = p.Name, Url = $"/san-pham/{p.Slug}" })
            .ToListAsync();
        var highlighted = await db.Products.Where(p => p.IsFeatured && p.IsActive).Take(4)
            .Select(p => new MegaMenuLinkViewModel { Label = p.Name, Url = $"/san-pham/{p.Slug}" })
            .ToListAsync();

        panels.Add(new MegaMenuPanelViewModel
        {
            CategoryKey = "san-pham-chon-loc",
            Title = "Sản phẩm chọn lọc",
            SeeAllUrl = "/danh-muc/san-pham-chon-loc",
            // Grey panel with no photo, like the other three non-"Quà tết" variants (node 908:15260).
            ImageUrl = null,
            Columns =
            [
                new MegaMenuColumnViewModel { Title = "Sản phẩm", Links = productTypes },
                new MegaMenuColumnViewModel { Title = "Bán chạy nhất", Links = topSellers },
                new MegaMenuColumnViewModel { Title = "Nổi bật", Links = highlighted },
            ],
        });

        return View(view, panels);
    }
}
