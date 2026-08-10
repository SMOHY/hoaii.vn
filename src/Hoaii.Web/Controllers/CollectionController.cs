using Hoaii.Infrastructure;
using Hoaii.Web.Models.Category;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Controllers;

/// <summary>Public landing page for one Collection — /bo-suu-tap/{slug}. Deliberately mirrors
/// CategoryController.Index (same product grid/sort/filter/paging, same hero-carousel-with-
/// admin-slide-override, same promo banner) and renders Category's own view file, since the
/// pages are meant to look and behave identically. Collections always use the Carousel hero style
/// and have no parent breadcrumb — the Banner style and breadcrumb nesting exist on Category only
/// to serve legacy occasion-listing pages that collections don't have.</summary>
public class CollectionController(HoaiiDbContext db) : Controller
{
    private const int PageSize = 9;

    public async Task<IActionResult> Index(string slug, int page = 1, string? sort = null,
                                            string? price = null, bool inStock = false)
    {
        var collection = await db.Collections.FirstOrDefaultAsync(c => c.Slug == slug);
        if (collection is null)
        {
            return NotFound();
        }

        IQueryable<Domain.Entities.Product> baseQuery = db.Products
            .Where(p => p.CollectionId == collection.Id && p.IsActive)
            .Include(p => p.Images)
            .Include(p => p.Variants);

        sort = CategoryController.SortOptions.Any(o => o.Key == sort) ? sort : CategoryController.SortOptions[0].Key;

        price = CategoryController.PriceFilters.Any(f => f.Key == price) ? price : "";
        if (price.Length > 0)
        {
            var band = CategoryController.PriceFilters.First(f => f.Key == price);
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
            "moi-nhat" => baseQuery.OrderByDescending(p => p.Badge == Domain.Entities.ProductBadge.New).ThenByDescending(p => p.Id),
            "gia-tang" => baseQuery.OrderBy(p => p.Price).ThenBy(p => p.Id),
            "gia-giam" => baseQuery.OrderByDescending(p => p.Price).ThenBy(p => p.Id),
            "ten-az" => baseQuery.OrderBy(p => p.Name).ThenBy(p => p.Id),
            _ => baseQuery.OrderBy(p => p.SortOrder).ThenByDescending(p => p.IsFeatured).ThenBy(p => p.Id),
        };

        var totalProducts = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalProducts / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var products = await query
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        // Admin-curated slides (Areas/Admin/Views/Collections/Edit.cshtml) fully replace the
        // auto-built ones once any exist for this collection — see CategoryController.Index for
        // the same pattern.
        var heroSlides = await db.CollectionHeroSlides
            .Where(s => s.CollectionId == collection.Id && s.IsActive)
            .OrderBy(s => s.SortOrder).ThenBy(s => s.Id)
            .Select(s => new HeroSlideViewModel { ImageUrl = s.ImageUrl, Name = s.Name, LinkUrl = s.LinkUrl })
            .ToListAsync();

        if (heroSlides.Count == 0)
        {
            heroSlides = await db.Products
                .Where(p => p.CollectionId == collection.Id && p.IsActive && p.Images.Any())
                .Include(p => p.Images)
                .OrderBy(p => p.SortOrder).ThenBy(p => p.Id)
                .Take(6)
                .Select(p => new HeroSlideViewModel
                {
                    ImageUrl = p.Images.OrderBy(i => i.SortOrder).First().Url,
                    Name = p.Name,
                    LinkUrl = $"/san-pham/{p.Slug}",
                })
                .ToListAsync();
        }

        var model = new CategoryPageViewModel
        {
            Title = collection.Name,
            BreadcrumbLabel = $"Trang chủ/{collection.Name}",
            Description = collection.Description is { Length: > 0 } d
                ? d : "Mỗi sản phẩm quà tặng đều mang một câu chuyện riêng",
            Products = products.Select(p => ProductCardMapper.Map(p)).ToList(),
            CurrentPage = page,
            TotalPages = totalPages,
            Slug = slug,
            BasePath = "/bo-suu-tap",
            Sort = sort,
            PriceFilter = price,
            InStockOnly = inStock,
            TotalProducts = totalProducts,
            HeroEyebrow = collection.HeroEyebrow is { Length: > 0 } he ? he : $"{collection.Name} đặc sắc",
            HeroKicker = collection.HeroKicker ?? "",
            HeroStyle = Domain.Entities.CategoryHeroStyle.Carousel,
            BannerImageUrl = null,
            Parent = null,
            HeroSlides = heroSlides,
            Promo = new PromoBannerViewModel
            {
                Eyebrow = collection.PromoEyebrow is { Length: > 0 } pe ? pe : "Hoài x Họa sĩ Lương Bình",
                Title = collection.PromoTitle is { Length: > 0 } pt ? pt
                    : "Bộ sưu tập được vẽ tay bởi họa sĩ Lương Bình — mỗi nét vẽ là một lát cắt văn hóa, mang câu chuyện di sản vào từng món quà.",
                CtaText = collection.PromoCtaText is { Length: > 0 } pct ? pct : "Mua ngay",
                CtaUrl = collection.PromoCtaUrl is { Length: > 0 } pcu ? pcu : $"/bo-suu-tap/{slug}",
                ImageUrl = collection.PromoImageUrl is { Length: > 0 } pi ? pi : "/images/category/promo-artist.jpg",
                Background = collection.PromoBackground,
                Wide = collection.PromoWide,
            },
        };

        return View("~/Views/Category/Index.cshtml", model);
    }
}
