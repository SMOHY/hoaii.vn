using Hoaii.Infrastructure;
using Hoaii.Web.Models.Category;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Controllers;

public class CategoryController(HoaiiDbContext db) : Controller
{
    // Figma's grid is 3 columns x 3 rows (node 722:25541, frames 235/236/237) and it ships the
    // pager hidden, because the 7 Quà tết products fit on one page. At 6 they spilled onto a
    // second page and the pager appeared, which the design never shows.
    private const int PageSize = 9;

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

    /// <summary>Price bands for the Bộ lọc panel. Key goes in the querystring; Min/Max are VND.
    /// Max is null for the open-ended top band — decimal.MaxValue overflows SQL Server's decimal.</summary>
    public static readonly IReadOnlyList<(string Key, string Label, decimal Min, decimal? Max)> PriceFilters =
    [
        ("duoi-500", "Dưới 500.000đ", 0m, 500_000m),
        ("500-1000", "500.000đ – 1.000.000đ", 500_000m, 1_000_000m),
        ("tren-1000", "Trên 1.000.000đ", 1_000_000m, null),
    ];

    public async Task<IActionResult> Index(string slug, int page = 1, string? sort = null,
                                           string? price = null, bool inStock = false)
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
        IQueryable<Domain.Entities.Product> baseQuery = (isFeaturedView
                ? db.Products.Where(p => p.IsFeatured && p.IsActive)
                : db.Products.Where(p => p.CategoryId == category!.Id && p.IsActive))
            .Include(p => p.Images)
            .Include(p => p.Variants);

        sort = SortOptions.Any(o => o.Key == sort) ? sort : SortOptions[0].Key;

        // Filtering happens before sorting and paging so the count and the pager stay honest.
        price = PriceFilters.Any(f => f.Key == price) ? price : "";
        if (price.Length > 0)
        {
            var band = PriceFilters.First(f => f.Key == price);
            baseQuery = baseQuery.Where(p => p.Price >= band.Min);
            if (band.Max is { } max)
            {
                baseQuery = baseQuery.Where(p => p.Price < max);
            }
        }
        if (inStock)
        {
            baseQuery = baseQuery.Where(p => p.Badge != Domain.Entities.ProductBadge.OutOfStock);
        }

        var query = sort switch
        {
            "moi-nhat" => baseQuery.OrderByDescending(p => p.Badge == Hoaii.Domain.Entities.ProductBadge.New).ThenByDescending(p => p.Id),
            "gia-tang" => baseQuery.OrderBy(p => p.Price).ThenBy(p => p.Id),
            "gia-giam" => baseQuery.OrderByDescending(p => p.Price).ThenBy(p => p.Id),
            "ten-az" => baseQuery.OrderBy(p => p.Name).ThenBy(p => p.Id),
            // SortOrder is the merchandiser's running order — Quà tết uses it to hold the exact
            // sequence Figma lays out. Everywhere else it is still 0, so those categories keep
            // falling through to featured-then-id as before.
            _ => baseQuery.OrderBy(p => p.SortOrder).ThenByDescending(p => p.IsFeatured).ThenBy(p => p.Id),
        };

        var totalProducts = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalProducts / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var products = await query
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        // The banner hero has no carousel, so skip the extra query for the eight pages that use it.
        var heroStyle = category?.HeroStyle ?? Domain.Entities.CategoryHeroStyle.Carousel;

        // The hero carousel shows this view's own products, not a separate asset set.
        var heroSlides = new List<HeroSlideViewModel>();
        if (heroStyle != Domain.Entities.CategoryHeroStyle.Banner)
        {
            heroSlides = await (isFeaturedView
                    ? db.Products.Where(p => p.IsFeatured && p.IsActive && p.Images.Any())
                    : db.Products.Where(p => p.CategoryId == category!.Id && p.IsActive && p.Images.Any()))
                .Include(p => p.Images)
                .OrderBy(p => p.SortOrder).ThenBy(p => p.Id)
                .Take(6)
                .Select(p => new HeroSlideViewModel
                {
                    ImageUrl = p.Images.OrderBy(i => i.SortOrder).First().Url,
                    Name = p.Name,
                    Slug = p.Slug,
                })
                .ToListAsync();
        }

        // Per-category CMS copy overrides these defaults; the cross-category "featured" view has no
        // Category row, so it always uses the defaults.
        var description = category?.Description is { Length: > 0 } d
            ? d : "Mỗi sản phẩm quà tặng đều mang một câu chuyện riêng";
        var heroEyebrow = category?.HeroEyebrow is { Length: > 0 } he
            ? he : $"{categoryName} đặc sắc";

        // No sensible default: the line names what is inside a set, which only the merchandiser
        // knows. Blank hides it rather than guessing a box count.
        var heroKicker = category?.HeroKicker ?? "";

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
            PriceFilter = price,
            InStockOnly = inStock,
            TotalProducts = totalProducts,
            HeroEyebrow = heroEyebrow,
            HeroKicker = heroKicker,
            HeroStyle = heroStyle,
            BannerImageUrl = category?.BannerImageUrl,
            Parent = category is { ParentLabel: { Length: > 0 } pl, ParentUrl: { Length: > 0 } pu }
                ? new BreadcrumbCrumb(pl, pu)
                : null,
            HeroSlides = heroSlides,
            Promo = new PromoBannerViewModel
            {
                Eyebrow = category?.PromoEyebrow is { Length: > 0 } pe ? pe : "Hoài x Họa sĩ Lương Bình",
                Title = category?.PromoTitle is { Length: > 0 } pt ? pt
                    : "Bộ sưu tập được vẽ tay bởi họa sĩ Lương Bình — mỗi nét vẽ là một lát cắt văn hóa, mang câu chuyện di sản vào từng món quà.",
                CtaText = category?.PromoCtaText is { Length: > 0 } pct ? pct : "Mua ngay",
                CtaUrl = category?.PromoCtaUrl is { Length: > 0 } pcu ? pcu : "/danh-muc/qua-tet",
                ImageUrl = category?.PromoImageUrl is { Length: > 0 } pi ? pi : "/images/category/promo-artist.jpg",
                Background = category?.PromoBackground,
                Wide = category?.PromoWide ?? false,
            },
        };

        return View(model);
    }
}
