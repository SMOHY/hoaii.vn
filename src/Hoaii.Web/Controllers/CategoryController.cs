using Hoaii.Infrastructure;
using Hoaii.Web.Models.Category;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Controllers;

public class CategoryController(HoaiiDbContext db) : Controller
{
    private const int PageSize = 9; // 3 columns x 3 rows desktop grid

    /// <summary>Nav and footer both link here, but it is a cross-category view of the
    /// featured products rather than a category row of its own.</summary>
    private const string FeaturedSlug = "san-pham-chon-loc";

    public async Task<IActionResult> Index(string slug, int page = 1)
    {
        var isFeaturedView = slug == FeaturedSlug;

        var category = isFeaturedView
            ? null
            : await db.Categories.FirstOrDefaultAsync(c => c.Slug == slug);

        if (!isFeaturedView && category is null)
        {
            return NotFound();
        }

        var categoryName = category?.Name ?? "Sản phẩm chọn lọc";

        var query = (isFeaturedView
                ? db.Products.Where(p => p.IsFeatured)
                : db.Products.Where(p => p.CategoryId == category!.Id))
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

        // The hero carousel shows this view's own products, not a separate asset set.
        var heroSlides = await (isFeaturedView
                ? db.Products.Where(p => p.IsFeatured && p.Images.Any())
                : db.Products.Where(p => p.CategoryId == category!.Id && p.Images.Any()))
            .Include(p => p.Images)
            .OrderBy(p => p.Id)
            .Take(6)
            .Select(p => new HeroSlideViewModel
            {
                ImageUrl = p.Images.OrderBy(i => i.SortOrder).First().Url,
                Name = p.Name,
                Slug = p.Slug,
            })
            .ToListAsync();

        var model = new CategoryPageViewModel
        {
            Title = categoryName,
            BreadcrumbLabel = $"Trang chủ/{categoryName}",
            Description = "Mỗi sản phẩm quà tặng đều mang một câu chuyện riêng",
            Products = products.Select(p => ProductCardMapper.Map(p)).ToList(),
            CurrentPage = page,
            TotalPages = totalPages,
            HeroEyebrow = $"{categoryName} đặc sắc",
            HeroSlides = heroSlides,
            Promo = new PromoBannerViewModel
            {
                Eyebrow = "Hoài x Họa sĩ Lương Bình",
                Title = "Bộ sưu tập được vẽ tay bởi họa sĩ Lương Bình — mỗi nét vẽ là một lát cắt văn hóa, mang câu chuyện di sản vào từng món quà.",
                CtaText = "Mua ngay",
                CtaUrl = "/danh-muc/qua-tet",
                ImageUrl = "/images/category/promo-artist.jpg",
            },
        };

        return View(model);
    }
}
