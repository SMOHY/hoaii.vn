using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Models.Occasion;
using Hoaii.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Controllers;

/// <summary>
/// The "Quà theo dịp" family of landing pages. These are not product grids — each one is a hero,
/// a three-way chooser, a few feature sections and a campaign band (nodes 769:12176 and
/// 778:22062). One action serves them all; <see cref="Pages"/> is the only thing that varies.
/// </summary>
public class OccasionController(HoaiiDbContext db) : Controller
{
    /// <summary>How many products a feature section shows. Figma draws two (node 769:15367).</summary>
    private const int ProductsPerSection = 2;

    /// <summary>A section is a category plus which side its cover sits on.</summary>
    private record SectionDef(string CategorySlug, string Title);

    private record PageDef(
        string Route,
        string Title,
        string HeroHeading,
        string HeroSubtitle,
        string MetaDescription,
        bool IsChildPage,
        IReadOnlyList<SectionDef> Sections);

    /// <summary>The three routes in the chooser (node 769:15244). Order matches Figma. Shared
    /// with MegaMenuViewComponent's "Quà tặng" column — see OccasionRoutes.</summary>
    private static readonly (string Title, string Route, string Thumb)[] ChooserRoutes = OccasionRoutes.ChooserRoutes;

    /// <summary>Only the active chooser column carries copy in Figma; the other two ship it hidden.</summary>
    private const string UmbrellaChooserCopy =
        "Mỗi dịp đặc biệt là một cơ hội để gửi trao yêu thương. Khám phá những món quà ý nghĩa dành cho các dấu mốc đáng nhớ.";

    private static readonly PageDef[] Pages =
    [
        new(
            Route: "qua-theo-dip",
            Title: "Quà tặng theo dịp",
            HeroHeading: "QUÀ TẶNG THEO DỊP",
            HeroSubtitle: "Trao gửi yêu thương đúng dịp",
            MetaDescription: "Quà tặng cho từng dịp trong năm — Valentine, Quốc tế Phụ nữ, Giáng sinh.",
            IsChildPage: false,
            Sections:
            [
                // Figma writes this one "Ngày lễ tình yêu- valentine" (node 769:15245); the mobile
                // frame has the clean form, so that is what ships.
                new("ngay-le-tinh-yeu", "Ngày lễ tình yêu"),
                new("ngay-quoc-te-phu-nu", "Ngày quốc tế phụ nữ"),
                // Figma typo "Qùa giáng sinh" (node 771:21273) corrected.
                new("qua-giang-sinh", "Quà giáng sinh"),
            ]),
        new(
            Route: "qua-tang-ca-nhan",
            Title: "Quà tặng cá nhân",
            // Figma repeats "QUÀ TẶNG THEO DỊP" here (node 1068:29891) — a leftover from
            // duplicating the umbrella frame. A page at /qua-tang-ca-nhan whose breadcrumb ends in
            // "Quà tặng cá nhân" cannot have that as its heading. See WF-013.
            HeroHeading: "QUÀ TẶNG CÁ NHÂN",
            HeroSubtitle: "Trao gửi yêu thương tới người thân",
            MetaDescription: "Quà tặng dành riêng cho người thân — người ấy và bố mẹ.",
            IsChildPage: true,
            Sections:
            [
                new("qua-tang-nguoi-ay", "Quà tặng người ấy"),
                new("qua-tang-bo-me", "Quà tặng bố mẹ"),
            ]),
    ];

    public async Task<IActionResult> Index(string route)
    {
        var page = Pages.FirstOrDefault(p => p.Route == route);
        if (page is null)
        {
            return NotFound();
        }

        // Which categories belong on this landing page used to be a hard-coded array — now it's
        // CategoryGroup, so an admin moving a category between groups actually changes what
        // renders here, not just the mega-menu. Falls back to the hard-coded list only if the
        // group row doesn't exist yet (e.g. CategoryGroupSeeder hasn't run).
        var group = await db.CategoryGroups
            .Include(g => g.Categories.OrderBy(c => c.SortOrder).ThenBy(c => c.Id))
            .FirstOrDefaultAsync(g => g.Route == route);
        var pageSections = group is not null
            ? group.Categories.Select(c => new SectionDef(c.Slug, c.Name)).ToList()
            : page.Sections.ToList();

        var slugs = pageSections.Select(s => s.CategorySlug).ToList();

        var categories = await db.Categories
            .Where(c => slugs.Contains(c.Slug))
            .ToDictionaryAsync(c => c.Slug);

        // One query for every section's products rather than one per section.
        var products = await db.Products
            .Where(p => p.IsActive && slugs.Contains(p.Category.Slug))
            .Include(p => p.Images)
            .Include(p => p.Category)
            .OrderBy(p => p.SortOrder).ThenBy(p => p.Id)
            .ToListAsync();

        var sections = new List<OccasionSectionViewModel>();
        for (var i = 0; i < pageSections.Count; i++)
        {
            var def = pageSections[i];
            categories.TryGetValue(def.CategorySlug, out var category);

            sections.Add(new OccasionSectionViewModel
            {
                Title = def.Title,
                Description = category?.Description ?? "",
                CoverImageUrl = category?.CoverImageUrl,
                // Figma alternates: cover right, cover left, cover right (nodes 769:15389,
                // 769:15390, 771:21270).
                CoverOnLeft = i % 2 == 1,
                ViewAllUrl = $"/danh-muc/{def.CategorySlug}",
                Products = products
                    .Where(p => p.Category.Slug == def.CategorySlug)
                    .Take(ProductsPerSection)
                    .Select(p => new CompactProductViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Slug = p.Slug,
                        Price = p.Price,
                        ImageUrl = p.Images.OrderBy(img => img.SortOrder).FirstOrDefault()?.Url,
                        IsOutOfStock = p.Badge == ProductBadge.OutOfStock,
                    })
                    .ToList(),
            });
        }

        var activeRoute = "/" + page.Route;
        var chooser = ChooserRoutes.Select(c => new OccasionChooserItemViewModel
        {
            Title = c.Title,
            Url = c.Route,
            ImageUrl = c.Thumb is { Length: > 0 } t ? t : null,
            IsActive = c.Route == activeRoute,
            Description = c.Route == activeRoute && !page.IsChildPage ? UmbrellaChooserCopy : "",
        }).ToList();

        List<BreadcrumbItem> breadcrumb = page.IsChildPage
            ? [new("Quà theo dịp", "/qua-theo-dip"), new(page.Title, null)]
            : [new(page.Title, null)];

        var model = new OccasionLandingViewModel
        {
            Title = page.Title,
            HeroHeading = page.HeroHeading,
            HeroSubtitle = page.HeroSubtitle,
            HeroImageUrl = "/images/occasions/landing/occasion-hero.jpg",
            MetaDescription = page.MetaDescription,
            Breadcrumb = breadcrumb,
            Chooser = chooser,
            Sections = sections,
            Campaign = new OccasionCampaignViewModel
            {
                // Figma leaves Lorem Ipsum here (node 771:15465). The artist collaboration is the
                // one campaign the site actually has copy for, so it is reused rather than
                // shipping placeholder Latin. See WF-012.
                Eyebrow = "Hoài x Họa sĩ Lương Bình",
                Body = "Bộ sưu tập được vẽ tay bởi họa sĩ Lương Bình — mỗi nét vẽ là một lát cắt văn hóa, mang câu chuyện di sản vào từng món quà.",
                CtaText = "Mua ngay",
                CtaUrl = "/danh-muc/qua-tet",
                ImageUrl = "/images/category/promo-artist.jpg",
                // Figma đặt nền hồng #E4C0D3 cho dải campaign của trang landing, khác hẳn các
                // trang danh mục (layer "sản phẩm nổi bật/giới hạn", file css all layer).
                Background = "#E4C0D3",
            },
        };

        return View(model);
    }
}
