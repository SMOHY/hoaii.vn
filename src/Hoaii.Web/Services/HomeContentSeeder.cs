using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Services;

/// <summary>
/// Seeds the homepage sections into the database the first time each table is empty, using the
/// exact copy/assets that used to live in HomeController. After that the admin owns them.
/// </summary>
public static class HomeContentSeeder
{
    public static async Task EnsureSeedAsync(HoaiiDbContext db)
    {
        if (!await db.HomeHeroSlides.AnyAsync())
        {
            db.HomeHeroSlides.Add(new HomeHeroSlide
            {
                ImageUrl = "/images/home/hero.jpg",
                Title = "TINH HOA VIỆT NAM",
                Subtitle = "Bộ sưu tập Quà tặng Trung Thu 2026",
                MobileTitle = "VIỆT NAM HOA THỊ",
                MobileSubtitle = "Concept tết mới nhất 2026",
                SortOrder = 0,
            });
        }

        if (!await db.HomeBenefits.AnyAsync())
        {
            db.HomeBenefits.AddRange(
                new HomeBenefit { IconPath = "/images/icons/benefit-shipping.png", Title = "Giao hàng toàn quốc", Description = "Thay bạn kết nối những tri âm, đưa quà đến từng ô cửa", MobileLine1 = "Giao hàng", MobileLine2 = "toàn quốc", SortOrder = 0 },
                new HomeBenefit { IconPath = "/images/icons/benefit-quality.png", Title = "Cam kết chất lượng", Description = "Tận tâm trong từng sản phẩm, an tâm tuyệt đối", MobileLine1 = "Cam kết", MobileLine2 = "chất lượng", SortOrder = 1 },
                new HomeBenefit { IconPath = "/images/icons/benefit-discount.png", Title = "Chiết khấu lên tới 35%", Description = "Giải pháp ngân sách tối ưu cho đơn hàng doanh nghiệp", MobileLine1 = "Chiết khấu", MobileLine2 = "lên tới 35%", SortOrder = 2 });
        }

        if (!await db.HomeFeaturedTiles.AnyAsync())
        {
            db.HomeFeaturedTiles.AddRange(
                new HomeFeaturedTile { IsCard = true, AccentColor = "red", TitleLine1 = "TINH HOA", TitleLine2 = "BẮC BỘ", LinkUrl = "/danh-muc/qua-tet", SortOrder = 0 },
                new HomeFeaturedTile { ImageUrl = "/images/placeholders/featured-2.jpg", LinkUrl = "/danh-muc/qua-tet", SortOrder = 1 },
                new HomeFeaturedTile { ImageUrl = "/images/placeholders/featured-3.jpg", HideOnMobile = true, LinkUrl = "/danh-muc/qua-tet", SortOrder = 2 },
                new HomeFeaturedTile { ImageUrl = "/images/placeholders/featured-4.jpg", LinkUrl = "/danh-muc/qua-tet", SortOrder = 3 },
                new HomeFeaturedTile { ImageUrl = "/images/placeholders/featured-5.jpg", HideOnMobile = true, LinkUrl = "/danh-muc/qua-tet", SortOrder = 4 },
                new HomeFeaturedTile { IsCard = true, AccentColor = "teal", TitleLine1 = "THIÊN ĐIỂU", TitleLine2 = "LẠC HỒNG", EditionLabel = "(Phiên bản cao cấp)", LinkUrl = "/danh-muc/qua-tet", SortOrder = 5 },
                new HomeFeaturedTile { IsCard = true, AccentColor = "yellow", TitleLine1 = "THIÊN ĐIỂU", TitleLine2 = "LẠC HỒNG", EditionLabel = "(Phiên bản thường)", LinkUrl = "/danh-muc/qua-tet", SortOrder = 6 },
                new HomeFeaturedTile { ImageUrl = "/images/placeholders/featured-6.jpg", LinkUrl = "/danh-muc/qua-tet", SortOrder = 7 },
                new HomeFeaturedTile { ImageUrl = "/images/placeholders/featured-7.jpg", HideOnMobile = true, LinkUrl = "/danh-muc/qua-tet", SortOrder = 8 });
        }

        if (!await db.HomeServiceTabs.AnyAsync())
        {
            db.HomeServiceTabs.AddRange(
                new HomeServiceTab { Key = "in-khac", Label = "In khắc logo cá nhân", IconSvg = "engraving", PanelImageUrl = "/images/home/service-panel.jpg", Caption = "Cá nhân hóa sản phẩm bằng logo, tên riêng của bạn.", CaptionColorHex = "#F2F2F2", CtaUrl = "/lien-he", SortOrder = 0 },
                new HomeServiceTab { Key = "goi-qua", Label = "Lựa chọn gói quà", IconSvg = "gift", PanelImageUrl = "/images/home/service-panel.jpg", Caption = "Tự do phối hợp gói quà theo sở thích và ngân sách", CaptionColorHex = "#F7E9EB", CtaUrl = "/lien-he", SortOrder = 1 },
                new HomeServiceTab { Key = "thiet-ke", Label = "Thiết kế ấn phẩm", IconSvg = "notepad-edit", PanelImageUrl = "/images/home/service-panel.jpg", Caption = "Ấn phẩm đi kèm được thiết kế riêng, độc bản", CaptionColorHex = "#F7E9EB", CtaUrl = "/lien-he", SortOrder = 2 });
        }

        if (!await db.HomeAboutCards.AnyAsync())
        {
            db.HomeAboutCards.AddRange(
                new HomeAboutCard { Caption = "Tính bản sắc", ImageOnTop = true, ImageUrl = "/images/home/about-ban-sac.jpg", SortOrder = 0 },
                new HomeAboutCard { Caption = "Sự tinh tế", ImageOnTop = false, ImageUrl = "/images/home/about-tinh-te.jpg", SortOrder = 1 },
                new HomeAboutCard { Caption = "Tư duy khởi sinh", ImageOnTop = true, ImageUrl = "/images/home/about-khoi-sinh.jpg", SortOrder = 2 },
                new HomeAboutCard { Caption = "Sự Tiếp nối", ImageOnTop = false, ImageUrl = "/images/home/about-tiep-noi.jpg", SortOrder = 3 });
        }

        if (!await db.HomeCustomerLogos.AnyAsync())
        {
            string[] logos =
            [
                "truong-thanh", "bondex", "nano-gold", "pro-group", "jaguar", "bee-mv",
                "core5", "cystack", "hana-hp", "mb", "nature-hotel", "avalue",
                "pancake", "ecomdy", "king-power", "isofh", "onpoint", "everest",
                "binh-minh-hp", "topcv", "saquila", "dht", "koni", "prime",
            ];
            for (var i = 0; i < logos.Length; i++)
            {
                db.HomeCustomerLogos.Add(new HomeCustomerLogo { LogoKey = logos[i], SortOrder = i });
            }
        }

        await db.SaveChangesAsync();
    }
}
