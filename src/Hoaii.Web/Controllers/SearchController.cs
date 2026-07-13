using Hoaii.Infrastructure;
using Hoaii.Web.Models.Category;
using Hoaii.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Controllers;

public class SearchController(HoaiiDbContext db) : Controller
{
    private const int GroupPreviewSize = 6; // 3 columns x 2 rows, matches design-specs/search-page.md

    public async Task<IActionResult> Index(string? q)
    {
        var query = (q ?? "").Trim();

        if (string.IsNullOrEmpty(query))
        {
            return View(new SearchPageViewModel { Query = query, TotalResultCount = 0, Groups = [] });
        }

        var matches = await db.Products
            .Where(p => p.Name.Contains(query))
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .ToListAsync();

        var groups = matches
            .GroupBy(p => p.Category)
            .Select(g => new SearchGroupViewModel
            {
                CategoryName = g.Key.Name,
                TotalCount = g.Count(),
                Products = g.Take(GroupPreviewSize).Select(p => ProductCardMapper.Map(p)).ToList(),
                ShowMoreUrl = $"/danh-muc/{g.Key.Slug}",
            })
            .OrderByDescending(g => g.TotalCount)
            .ToList();

        // "Sản phẩm chọn lọc" fallback/cross-sell block — see design-specs/search-page.md.
        var matchedIds = matches.Select(p => p.Id).ToList();
        var featured = await db.Products
            .Where(p => p.IsFeatured && !matchedIds.Contains(p.Id))
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .Take(GroupPreviewSize)
            .ToListAsync();

        if (featured.Count > 0)
        {
            groups.Add(new SearchGroupViewModel
            {
                CategoryName = "Sản phẩm chọn lọc",
                TotalCount = featured.Count,
                Products = featured.Select(p => ProductCardMapper.Map(p)).ToList(),
                ShowMoreUrl = "/",
                IsFallback = true,
            });
        }

        var model = new SearchPageViewModel
        {
            Query = query,
            TotalResultCount = matches.Count,
            Groups = groups,
        };

        return View(model);
    }
}
