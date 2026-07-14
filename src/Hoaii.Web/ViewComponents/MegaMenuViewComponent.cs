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
                .Where(p => p.CategoryId == category.Id)
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

            var panelImage = await db.ProductImages
                .Where(i => i.Product.CategoryId == category.Id)
                .OrderBy(i => i.ProductId).ThenBy(i => i.SortOrder)
                .Select(i => i.Url)
                .FirstOrDefaultAsync();

            panels.Add(new MegaMenuPanelViewModel
            {
                CategoryKey = slug,
                SeeAllUrl = $"/danh-muc/{slug}",
                ImageUrl = panelImage,
                Columns =
                [
                    new MegaMenuColumnViewModel { Title = "Bán chạy nhất", Links = bestSellers },
                    new MegaMenuColumnViewModel { Title = "Phiên bản giới hạn", Links = limited },
                    new MegaMenuColumnViewModel { Title = "Theo bộ sưu tập", Links = otherOccasions },
                ],
            });
        }

        // "Sản phẩm chọn lọc" has no literal Category row — it's a cross-category
        // featured/new view, so its columns are sourced differently from the occasion panels above.
        var productTypeCategories = await db.Categories
            .Where(c => c.Type == CategoryType.ProductType)
            .ToListAsync();
        var featured = await db.Products.Where(p => p.IsFeatured).Take(4)
            .Select(p => new MegaMenuLinkViewModel { Label = p.Name, Url = $"/san-pham/{p.Slug}" })
            .ToListAsync();
        var newest = await db.Products.Where(p => p.Badge == ProductBadge.New).Take(4)
            .Select(p => new MegaMenuLinkViewModel { Label = p.Name, Url = $"/san-pham/{p.Slug}" })
            .ToListAsync();

        var featuredImage = await db.ProductImages
            .Where(i => i.Product.IsFeatured)
            .OrderBy(i => i.ProductId).ThenBy(i => i.SortOrder)
            .Select(i => i.Url)
            .FirstOrDefaultAsync();

        panels.Add(new MegaMenuPanelViewModel
        {
            CategoryKey = "san-pham-chon-loc",
            SeeAllUrl = "/danh-muc/san-pham-chon-loc",
            ImageUrl = featuredImage,
            Columns =
            [
                new MegaMenuColumnViewModel { Title = "Bán chạy nhất", Links = featured },
                new MegaMenuColumnViewModel { Title = "Phiên bản giới hạn", Links = newest },
                new MegaMenuColumnViewModel
                {
                    Title = "Theo bộ sưu tập",
                    Links = productTypeCategories.Select(c => new MegaMenuLinkViewModel { Label = c.Name, Url = $"/danh-muc/{c.Slug}" }).ToList(),
                },
            ],
        });

        return View(view, panels);
    }
}
