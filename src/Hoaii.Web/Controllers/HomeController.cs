using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hoaii.Infrastructure;
using Hoaii.Web.Models;
using Hoaii.Web.Models.Home;

namespace Hoaii.Web.Controllers;

public class HomeController(HoaiiDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        // The blog strip reads real published posts: the featured one first, then the three
        // most recent. Empty DB → empty list and the home view hides the whole section.
        var recent = await db.BlogPosts
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.IsFeatured)
            .ThenByDescending(p => p.PublishedAt)
            .Take(4)
            .Select(p => new BlogPostViewModel
            {
                Category = p.Category,
                Title = p.Title,
                Excerpt = p.Excerpt,
                Url = "/blog/" + p.Slug,
                ImageUrl = p.ImageUrl ?? "/images/placeholders/blog-1.jpg",
                IsFeatured = p.IsFeatured,
            })
            .ToListAsync();

        var model = new HomeIndexViewModel
        {
            HeroSlides = await db.HomeHeroSlides
                .Where(h => h.IsActive).OrderBy(h => h.SortOrder).ThenBy(h => h.Id)
                .Select(h => new HeroSlideViewModel
                {
                    ImageUrl = h.ImageUrl,
                    Title = h.Title,
                    Subtitle = h.Subtitle,
                    MobileTitle = h.MobileTitle,
                    MobileSubtitle = h.MobileSubtitle,
                })
                .ToListAsync(),

            Benefits = await db.HomeBenefits
                .OrderBy(b => b.SortOrder).ThenBy(b => b.Id)
                .Select(b => new BenefitViewModel
                {
                    IconPath = b.IconPath,
                    Title = b.Title,
                    Description = b.Description,
                    MobileLine1 = b.MobileLine1,
                    MobileLine2 = b.MobileLine2,
                })
                .ToListAsync(),

            FeaturedTiles = await db.HomeFeaturedTiles
                .OrderBy(t => t.SortOrder).ThenBy(t => t.Id)
                .Select(t => new FeaturedTileViewModel
                {
                    IsCard = t.IsCard,
                    AccentColor = t.AccentColor,
                    CollectionLabel = t.CollectionLabel,
                    TitleLine1 = t.TitleLine1,
                    TitleLine2 = t.TitleLine2,
                    EditionLabel = t.EditionLabel,
                    HideOnMobile = t.HideOnMobile,
                    ImageUrl = t.ImageUrl,
                    LinkUrl = t.LinkUrl,
                })
                .ToListAsync(),

            CustomServiceTabs = await db.HomeServiceTabs
                .OrderBy(s => s.SortOrder).ThenBy(s => s.Id)
                .Select(s => new CustomServiceTabViewModel
                {
                    Key = s.Key,
                    Label = s.Label,
                    IconSvg = s.IconSvg,
                    PanelImageUrl = s.PanelImageUrl,
                    Caption = s.Caption,
                    CaptionColorHex = s.CaptionColorHex,
                    CtaText = s.CtaText,
                    CtaUrl = s.CtaUrl,
                })
                .ToListAsync(),

            AboutCards = await db.HomeAboutCards
                .OrderBy(a => a.SortOrder).ThenBy(a => a.Id)
                .Select(a => new AboutCardViewModel
                {
                    Caption = a.Caption,
                    ImageOnTop = a.ImageOnTop,
                    ImageUrl = a.ImageUrl,
                })
                .ToListAsync(),

            CustomerLogos = await db.HomeCustomerLogos
                .OrderBy(l => l.SortOrder).ThenBy(l => l.Id)
                .Select(l => l.LogoKey)
                .ToListAsync(),

            BlogPosts = recent,
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
