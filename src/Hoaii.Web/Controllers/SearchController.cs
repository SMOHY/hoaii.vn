using System.Globalization;
using System.Text;
using Hoaii.Infrastructure;
using Hoaii.Web.Models.Category;
using Hoaii.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Controllers;

public class SearchController(HoaiiDbContext db) : Controller
{
    private const int GroupPreviewSize = 6; // 3 columns x 2 rows, matches design-specs/search-page.md

    /// <summary>
    /// "Thiên điểu" -> "thien-dieu". Slugs are stored unaccented, so matching the query against
    /// them lets people type without diacritics — which is how most Vietnamese search.
    /// </summary>
    private static string Slugify(string value)
    {
        var decomposed = value.Trim().ToLowerInvariant().Replace('đ', 'd').Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(decomposed.Length);

        foreach (var c in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }
            sb.Append(char.IsLetterOrDigit(c) ? c : '-');
        }

        return string.Join('-', sb.ToString().Split('-', StringSplitOptions.RemoveEmptyEntries));
    }

    public async Task<IActionResult> Index(string? q)
    {
        var query = (q ?? "").Trim();

        if (string.IsNullOrEmpty(query))
        {
            return View(new SearchPageViewModel { Query = query, TotalResultCount = 0, Groups = [] });
        }

        var slugQuery = Slugify(query);

        var matches = await db.Products
            .Where(p => p.IsActive
                        && (p.Name.Contains(query)
                            || (slugQuery.Length > 0 && p.Slug.Contains(slugQuery))))
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
            .Where(p => p.IsFeatured && p.IsActive && !matchedIds.Contains(p.Id))
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
