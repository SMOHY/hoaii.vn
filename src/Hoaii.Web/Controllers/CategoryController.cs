using Hoaii.Infrastructure;
using Hoaii.Web.Models.Category;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Controllers;

public class CategoryController(HoaiiDbContext db) : Controller
{
    // Figma lays the grid out as 3 columns x 2 rows and then pages (node 1519:34031).
    // At 9 a category with 6-9 products never showed the pager at all.
    private const int PageSize = 6;

    /// <summary>Nav and footer both link here, but it is a cross-category view of the
    /// featured products rather than a category row of its own.</summary>
    private const string FeaturedSlug = "san-pham-chon-loc";

    /// <summary>Keys are what appears in the querystring; the labels feed the sort dropdown.</summary>
    public static readonly IReadOnlyList<(string Key, string Label)> SortOptions =
    [
        ("noi-bat", "Nổi bật"),
        ("moi-nhat", "Mới nhất"),
        ("gia-tang", "Giá: thấp đến cao"),
        ("gia-giam", "Giá: cao đến thấp"),
        ("ten-az", "Tên: A-Z"),
    ];

    public async Task<IActionResult> Index(string slug, int page = 1, string? sort = null)
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

        // Hidden products never reach the storefront.
        var baseQuery = (isFeaturedView
                ? db.Products.Where(p => p.IsFeatured && p.IsActive)
                : db.Products.Where(p => p.CategoryId == category!.Id && p.IsActive))
            .Include(p => p.Images)
            .Include(p => p.Variants);

        sort = SortOptions.Any(o => o.Key == sort) ? sort : SortOptions[0].Key;

        var query = sort switch
        {
            "moi-nhat" => baseQuery.OrderByDescending(p => p.Badge == Hoaii.Domain.Entities.ProductBadge.New).ThenByDescending(p => p.Id),
            "gia-tang" => baseQuery.OrderBy(p => p.Price).ThenBy(p => p.Id),
            "gia-giam" => baseQuery.OrderByDescending(p => p.Price).ThenBy(p => p.Id),
            "ten-az" => baseQuery.OrderBy(p => p.Name).ThenBy(p => p.Id),
            _ => baseQuery.OrderByDescending(p => p.IsFeatured).ThenBy(p => p.Id),
        };

        var totalProducts = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalProducts / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var products = await query
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        // The hero carousel shows this view's own products, not a separate asset set.
        var heroSlides = await (isFeaturedView
                ? db.Products.Where(p => p.IsFeatured && p.IsActive && p.Images.Any())
                : db.Products.Where(p => p.CategoryId == category!.Id && p.IsActive && p.Images.Any()))
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

        // Per-category CMS copy overrides these defaults; the cross-category "featured" view has no
        // Category row, so it always uses the defaults.
        var description = category?.Description is { Length: > 0 } d
            ? d : "Mỗi sản phẩm quà tặng đều mang một câu chuyện riêng";
        var heroEyebrow = category?.HeroEyebrow is { Length: > 0 } he
            ? he : $"{categoryName} đặc sắc";

        var model = new CategoryPageViewModel
        {
            Title = categoryName,
            BreadcrumbLabel = $"Trang chủ/{categoryName}",
            Description = description,
            Products = products.Select(p => ProductCardMapper.Map(p)).ToList(),
            CurrentPage = page,
            TotalPages = totalPages,
            Slug = slug,
            Sort = sort,
            TotalProducts = totalProducts,
            HeroEyebrow = heroEyebrow,
            HeroSlides = heroSlides,
            Promo = new PromoBannerViewModel
            {
                Eyebrow = category?.PromoEyebrow is { Length: > 0 } pe ? pe : "Hoài x Họa sĩ Lương Bình",
                Title = category?.PromoTitle is { Length: > 0 } pt ? pt
                    : "Bộ sưu tập được vẽ tay bởi họa sĩ Lương Bình — mỗi nét vẽ là một lát cắt văn hóa, mang câu chuyện di sản vào từng món quà.",
                CtaText = category?.PromoCtaText is { Length: > 0 } pct ? pct : "Mua ngay",
                CtaUrl = category?.PromoCtaUrl is { Length: > 0 } pcu ? pcu : "/danh-muc/qua-tet",
                ImageUrl = category?.PromoImageUrl is { Length: > 0 } pi ? pi : "/images/category/promo-artist.jpg",
            },
        };

        return View(model);
    }
}
