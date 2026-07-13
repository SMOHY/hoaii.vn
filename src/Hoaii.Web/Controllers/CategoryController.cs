using Hoaii.Infrastructure;
using Hoaii.Web.Models.Category;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Controllers;

public class CategoryController(HoaiiDbContext db) : Controller
{
    private const int PageSize = 9; // 3 columns x 3 rows desktop grid

    public async Task<IActionResult> Index(string slug, int page = 1)
    {
        var category = await db.Categories
            .FirstOrDefaultAsync(c => c.Slug == slug);

        if (category is null)
        {
            return NotFound();
        }

        var query = db.Products
            .Where(p => p.CategoryId == category.Id)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .OrderBy(p => p.Id);

        var totalProducts = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalProducts / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var products = await query
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        var model = new CategoryPageViewModel
        {
            Title = category.Name,
            BreadcrumbLabel = $"Trang chủ/{category.Name}",
            Description = $"Khám phá bộ sưu tập {category.Name.ToLowerInvariant()} được HOÀI tuyển chọn kỹ lưỡng.",
            Products = products.Select(p => ProductCardMapper.Map(p)).ToList(),
            CurrentPage = page,
            TotalPages = totalPages,
            Promo = new PromoBannerViewModel
            {
                Eyebrow = "Sản phẩm giới hạn",
                Title = $"Bộ sưu tập {category.Name} phiên bản đặc biệt, số lượng có hạn trong dịp này.",
                CtaText = "Mua ngay",
                CtaUrl = "#",
            },
        };

        return View(model);
    }
}
