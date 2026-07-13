using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Hoaii.Web.Models;
using Hoaii.Web.Models.Home;

namespace Hoaii.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var model = new HomeIndexViewModel
        {
            Benefits =
            [
                new() { IconPath = "/images/icons/benefit-shipping.svg", Title = "Miễn phí giao hàng", Description = "Trao tận tay bạn, miễn phí trên toàn quốc", MobileLine1 = "Miễn phí", MobileLine2 = "giao hàng" },
                new() { IconPath = "/images/icons/benefit-quality.svg", Title = "Cam kết chất lượng", Description = "Tận tâm trong từng sản phẩm, an tâm tuyệt đối", MobileLine1 = "Cam kết", MobileLine2 = "chất lượng" },
                new() { IconPath = "/images/icons/benefit-discount.svg", Title = "Chiết khấu tới 30%", Description = "Đặc quyền ưu đãi dành riêng cho bạn", MobileLine1 = "Chiết khấu", MobileLine2 = "tới 30%" },
            ],
            FeaturedTiles =
            [
                // Row 1
                new() { IsCard = true, AccentColor = "red", CollectionLabel = "Bộ sưu tập", TitleLine1 = "TINH HOA", TitleLine2 = "BẮC BỘ", LinkUrl = "/danh-muc/qua-tet" },
                new() { ImageUrl = "/images/placeholders/featured-2.jpg" },
                new() { ImageUrl = "/images/placeholders/featured-3.jpg" },
                // Row 2
                new() { ImageUrl = "/images/placeholders/featured-4.jpg" },
                new() { ImageUrl = "/images/placeholders/featured-5.jpg" },
                new() { IsCard = true, AccentColor = "teal", CollectionLabel = "Bộ sưu tập", TitleLine1 = "THIÊN ĐIỂU", TitleLine2 = "LẠC HỒNG", LinkUrl = "/danh-muc/qua-tet" },
                // Row 3
                new() { IsCard = true, AccentColor = "gold", CollectionLabel = "Bộ sưu tập", TitleLine1 = "DÂN GIAN", TitleLine2 = "HỌA KỲ", LinkUrl = "/danh-muc/qua-tet" },
                new() { ImageUrl = "/images/placeholders/featured-6.jpg" },
                new() { ImageUrl = "/images/placeholders/featured-7.jpg" },
            ],
            CustomServiceTabs =
            [
                new()
                {
                    Key = "in-khac",
                    Label = "In khắc logo cá nhân",
                    IconSvg = "engraving",
                    PanelImageUrl = "/images/placeholders/service-in-khac.jpg",
                    Caption = "Cá nhân hóa sản phẩm bằng logo, tên riêng của bạn.",
                    CaptionColorHex = "#F2F2F2",
                    CtaUrl = "/dich-vu/in-khac",
                },
                new()
                {
                    Key = "goi-qua",
                    Label = "Lựa chọn gói quà",
                    IconSvg = "gift",
                    PanelImageUrl = "/images/placeholders/service-goi-qua.jpg",
                    Caption = "Tự do phối hợp gói quà theo sở thích và ngân sách",
                    CaptionColorHex = "#F7E9EB",
                    CtaUrl = "/dich-vu/lua-chon-goi-qua",
                },
                new()
                {
                    Key = "thiet-ke",
                    Label = "Thiết kế ấn phẩm",
                    IconSvg = "notepad-edit",
                    PanelImageUrl = "/images/placeholders/service-thiet-ke.jpg",
                    Caption = "Ấn phẩm đi kèm được thiết kế riêng, độc bản",
                    CaptionColorHex = "#F7E9EB",
                    CtaUrl = "/dich-vu/thiet-ke",
                },
            ],
            AboutCards =
            [
                new() { Caption = "Tính bản sắc", ImageOnTop = true },
                new() { Caption = "Sự tinh tế", ImageOnTop = false },
                new() { Caption = "Tư duy khởi sinh", ImageOnTop = true },
                new() { Caption = "Sự Tiếp nối", ImageOnTop = false },
            ],
            CustomerLogos =
            [
                "truong-thanh", "bondex", "nano-gold", "pro-group", "jaguar", "bee-mv",
                "core5", "cystack", "hana-hp", "mb", "nature-hotel", "avalue",
                "pancake", "ecomdy", "king-power", "isofh", "onpoint", "everest",
            ],
            BlogPosts =
            [
                new() { IsFeatured = true, Category = "Đời sống", Title = "Gợi ý chọn quà tặng cho người thân yêu", Excerpt = "Lorem ipsum dolor sit amet consectetur. Eu id et commodo pharetra habitasse. Massa odio tincidunt consequat sed nulla sit.", Url = "/blog/goi-y-chon-qua-tang", ImageUrl = "/images/placeholders/blog-1.jpg" },
                new() { Category = "Đời sống", Title = "Gợi ý chọn quà tặng cho người thân yêu", Excerpt = "Lorem ipsum dolor sit amet consectetur. Eu id et commodo pharetra habitasse.", Url = "/blog/bai-viet-2", ImageUrl = "/images/placeholders/blog-2.jpg" },
                new() { Category = "Đời sống", Title = "Gợi ý chọn quà tặng cho người thân yêu", Excerpt = "Lorem ipsum dolor sit amet consectetur. Eu id et commodo pharetra habitasse.", Url = "/blog/bai-viet-3", ImageUrl = "/images/placeholders/blog-3.jpg" },
                new() { Category = "Đời sống", Title = "Gợi ý chọn quà tặng cho người thân yêu", Excerpt = "Lorem ipsum dolor sit amet consectetur. Eu id et commodo pharetra habitasse.", Url = "/blog/bai-viet-4", ImageUrl = "/images/placeholders/blog-4.jpg" },
            ],
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
