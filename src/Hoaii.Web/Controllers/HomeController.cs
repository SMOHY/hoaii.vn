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
            // Only one hero image has been delivered so far. Adding a second entry here is all
            // it takes to turn the arrows and dots back on — see HomeIndexViewModel.HeroSlides.
            HeroSlides =
            [
                new()
                {
                    ImageUrl = "/images/home/hero.jpg",
                    Title = "TINH HOA VIỆT NAM",
                    Subtitle = "Bộ sưu tập Quà tặng Trung Thu 2026",
                    MobileTitle = "VIỆT NAM HOA THỊ",
                    MobileSubtitle = "Concept tết mới nhất 2026",
                },
            ],
            Benefits =
            [
                // Illustrated icons exported from Figma (nodes 1214:38763 / 38822 / 38847) —
                // the old SVGs were plain line icons, not the drawn artwork.
                new() { IconPath = "/images/icons/benefit-shipping.png", Title = "Giao hàng toàn quốc", Description = "Thay bạn kết nối những tri âm, đưa quà đến từng ô cửa", MobileLine1 = "Giao hàng", MobileLine2 = "toàn quốc" },
                new() { IconPath = "/images/icons/benefit-quality.png", Title = "Cam kết chất lượng", Description = "Tận tâm trong từng sản phẩm, an tâm tuyệt đối", MobileLine1 = "Cam kết", MobileLine2 = "chất lượng" },
                new() { IconPath = "/images/icons/benefit-discount.png", Title = "Chiết khấu lên tới 35%", Description = "Giải pháp ngân sách tối ưu cho đơn hàng doanh nghiệp", MobileLine1 = "Chiết khấu", MobileLine2 = "lên tới 35%" },
            ],
            FeaturedTiles =
            [
                // Row 1 — red "TINH HOA BẮC BỘ" card, left (node 1214:38738)
                new() { IsCard = true, AccentColor = "red", TitleLine1 = "TINH HOA", TitleLine2 = "BẮC BỘ", LinkUrl = "/danh-muc/qua-tet" },
                new() { ImageUrl = "/images/placeholders/featured-2.jpg" },
                new() { ImageUrl = "/images/placeholders/featured-3.jpg", HideOnMobile = true },
                // Row 2 — teal card, right (node 1214:38752)
                new() { ImageUrl = "/images/placeholders/featured-4.jpg" },
                new() { ImageUrl = "/images/placeholders/featured-5.jpg", HideOnMobile = true },
                new() { IsCard = true, AccentColor = "teal", TitleLine1 = "THIÊN ĐIỂU", TitleLine2 = "LẠC HỒNG", EditionLabel = "(Phiên bản cao cấp)", LinkUrl = "/danh-muc/qua-tet" },
                // Row 3 — yellow card, left (node 1417:39224)
                new() { IsCard = true, AccentColor = "yellow", TitleLine1 = "THIÊN ĐIỂU", TitleLine2 = "LẠC HỒNG", EditionLabel = "(Phiên bản thường)", LinkUrl = "/danh-muc/qua-tet" },
                new() { ImageUrl = "/images/placeholders/featured-6.jpg" },
                new() { ImageUrl = "/images/placeholders/featured-7.jpg", HideOnMobile = true },
            ],
            CustomServiceTabs =
            [
                new()
                {
                    Key = "in-khac",
                    Label = "In khắc logo cá nhân",
                    IconSvg = "engraving",
                    PanelImageUrl = "/images/home/service-panel.jpg",
                    Caption = "Cá nhân hóa sản phẩm bằng logo, tên riêng của bạn.",
                    CaptionColorHex = "#F2F2F2",
                    CtaUrl = "/dich-vu/in-khac",
                },
                new()
                {
                    Key = "goi-qua",
                    Label = "Lựa chọn gói quà",
                    IconSvg = "gift",
                    PanelImageUrl = "/images/home/service-panel.jpg",
                    Caption = "Tự do phối hợp gói quà theo sở thích và ngân sách",
                    CaptionColorHex = "#F7E9EB",
                    CtaUrl = "/dich-vu/lua-chon-goi-qua",
                },
                new()
                {
                    Key = "thiet-ke",
                    Label = "Thiết kế ấn phẩm",
                    IconSvg = "notepad-edit",
                    PanelImageUrl = "/images/home/service-panel.jpg",
                    Caption = "Ấn phẩm đi kèm được thiết kế riêng, độc bản",
                    CaptionColorHex = "#F7E9EB",
                    CtaUrl = "/dich-vu/thiet-ke",
                },
            ],
            AboutCards =
            [
                new() { Caption = "Tính bản sắc", ImageOnTop = true, ImageUrl = "/images/home/about-ban-sac.jpg" },
                new() { Caption = "Sự tinh tế", ImageOnTop = false, ImageUrl = "/images/home/about-tinh-te.jpg" },
                new() { Caption = "Tư duy khởi sinh", ImageOnTop = true, ImageUrl = "/images/home/about-khoi-sinh.jpg" },
                new() { Caption = "Sự Tiếp nối", ImageOnTop = false, ImageUrl = "/images/home/about-tiep-noi.jpg" },
            ],
            // The 24 logos Figma runs across the three marquee rows, in order (node 792:24979).
            CustomerLogos =
            [
                "truong-thanh", "bondex", "nano-gold", "pro-group", "jaguar", "bee-mv",
                "core5", "cystack", "hana-hp", "mb", "nature-hotel", "avalue",
                "pancake", "ecomdy", "king-power", "isofh", "onpoint", "everest",
                "binh-minh-hp", "topcv", "saquila", "dht", "koni", "prime",
            ],
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
