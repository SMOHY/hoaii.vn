using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Services;

/// <summary>Seeds the header/footer menus into the DB the first time they're empty, from the copy
/// that used to live in LayoutData.</summary>
public static class NavigationSeeder
{
    public static async Task EnsureSeedAsync(HoaiiDbContext db)
    {
        // Phải chạy trước khi thêm NavLink: mục menu trỏ tới /danh-muc/{slug} mà không có row
        // Categories khớp slug thì CategoryController.Index trả NotFound, bấm vào là 404.
        await EnsureBirthdayCakeCategoryAsync(db);

        if (!await db.NavLinks.AnyAsync())
        {
            db.NavLinks.AddRange(
                new NavLink { Placement = NavPlacement.Main, Label = "Quà tết", Url = "/danh-muc/qua-tet", HasDropdown = true, SortOrder = 0 },
                new NavLink { Placement = NavPlacement.Main, Label = "Quà trung thu", Url = "/danh-muc/qua-trung-thu", HasDropdown = true, SortOrder = 1 },
                new NavLink { Placement = NavPlacement.Main, Label = "Quà theo dịp", Url = "/qua-theo-dip", HasDropdown = true, SortOrder = 2 },
                new NavLink { Placement = NavPlacement.Main, Label = "Sản phẩm chọn lọc", Url = "/danh-muc/san-pham-chon-loc", HasDropdown = true, SortOrder = 3 },
                // HasDropdown = false có chủ đích: MegaMenuViewComponent chỉ dựng panel cho mục
                // tuỳ chỉnh khi nó đã có NavLink.Children. Bật dropdown lúc chưa có submenu thì
                // nav render trigger nhưng không có panel, bấm vào không mở ra gì.
                new NavLink { Placement = NavPlacement.Main, Label = "Bánh sinh nhật", Url = "/danh-muc/banh-sinh-nhat", HasDropdown = false, SortOrder = 4 },
                new NavLink { Placement = NavPlacement.Sub, Label = "Về chúng tôi", Url = "/ve-chung-toi", SortOrder = 0 },
                new NavLink { Placement = NavPlacement.Sub, Label = "Liên hệ", Url = "/lien-he", SortOrder = 1 },
                new NavLink { Placement = NavPlacement.Sub, Label = "Đại lý", Url = "/hop-tac", SortOrder = 2 },
                new NavLink { Placement = NavPlacement.Sub, Label = "Blog", Url = "/blog", SortOrder = 3 });
        }
        else
        {
            await EnsureMainNavLinkAsync(db, "Bánh sinh nhật", "/danh-muc/banh-sinh-nhat", 4);
        }

        if (!await db.FooterMenuColumns.AnyAsync())
        {
            db.FooterMenuColumns.AddRange(
                new FooterMenuColumn
                {
                    Title = "VỀ HOÀI", SortOrder = 0,
                    Links =
                    [
                        new() { Label = "Quà tết", Url = "/danh-muc/qua-tet", SortOrder = 0 },
                        new() { Label = "Quà trung thu", Url = "/danh-muc/qua-trung-thu", SortOrder = 1 },
                        new() { Label = "Quà theo dịp", Url = "/qua-theo-dip", SortOrder = 2 },
                        new() { Label = "Sản phẩm chọn lọc", Url = "/danh-muc/san-pham-chon-loc", SortOrder = 3 },
                        new() { Label = "Câu chuyện", Url = "/blog", SortOrder = 4 },
                        new() { Label = "Đối tác", Url = "/hop-tac", SortOrder = 5 },
                        new() { Label = "Bánh sinh nhật", Url = "/danh-muc/banh-sinh-nhat", SortOrder = 6 },
                    ],
                },
                new FooterMenuColumn
                {
                    Title = "HỖ TRỢ KHÁCH HÀNG", SortOrder = 1,
                    Links =
                    [
                        new() { Label = "Liên hệ", Url = "/lien-he", SortOrder = 0 },
                        new() { Label = "Chính sách trao đổi & hoàn tác", Url = "/chinh-sach/trao-doi", SortOrder = 1 },
                        new() { Label = "Chính sách giao nhận hàng hóa", Url = "/chinh-sach/giao-hang", SortOrder = 2 },
                        new() { Label = "Chính sách giá & thanh toán", Url = "/chinh-sach/gia-thanh-toan", SortOrder = 3 },
                        new() { Label = "Giải quyết khiếu nại", Url = "/chinh-sach/khieu-nai", SortOrder = 4 },
                    ],
                },
                new FooterMenuColumn
                {
                    Title = "CHÍNH SÁCH PHÁP LÝ", SortOrder = 2,
                    Links =
                    [
                        new() { Label = "Điều khoản sử dụng", Url = "/chinh-sach/dieu-khoan-su-dung", SortOrder = 0 },
                        new() { Label = "Chính sách bảo vệ dữ liệu cá nhân", Url = "/chinh-sach/bao-mat", SortOrder = 1 },
                        new() { Label = "Thông tin chủ sở hữu", Url = "/chinh-sach/thong-tin-chu-so-huu", SortOrder = 2 },
                    ],
                });
        }
        else
        {
            await EnsureFooterLinkAsync(db, "HỖ TRỢ KHÁCH HÀNG", "Chính sách giá & thanh toán", "/chinh-sach/gia-thanh-toan", 3);
            await EnsureFooterLinkAsync(db, "HỖ TRỢ KHÁCH HÀNG", "Giải quyết khiếu nại", "/chinh-sach/khieu-nai", 4);
            await EnsureFooterLinkAsync(db, "CHÍNH SÁCH PHÁP LÝ", "Thông tin chủ sở hữu", "/chinh-sach/thong-tin-chu-so-huu", 2);
            await EnsureFooterLinkAsync(db, "VỀ HOÀI", "Bánh sinh nhật", "/danh-muc/banh-sinh-nhat", 6);
        }

        await db.SaveChangesAsync();
    }

    /// <summary>Adds a footer link to an existing column if a link with that URL isn't already
    /// there — lets new pages reach a footer that was seeded before they existed.</summary>
    private static async Task EnsureFooterLinkAsync(HoaiiDbContext db, string columnTitle, string label, string url, int sortOrder)
    {
        var column = await db.FooterMenuColumns
            .Include(c => c.Links)
            .FirstOrDefaultAsync(c => c.Title == columnTitle);
        if (column is null || column.Links.Any(l => l.Url == url))
        {
            return;
        }

        column.Links.Add(new FooterMenuLink { Label = label, Url = url, SortOrder = sortOrder });
    }

    /// <summary>Cùng lý do như EnsureFooterLinkAsync, nhưng cho menu chính: DB đã seed từ trước
    /// không đi qua nhánh AddRange, nên mục menu mới sẽ không bao giờ xuất hiện nếu không thêm
    /// riêng ở đây. Đối chiếu theo URL nên chạy lại nhiều lần cũng không nhân bản.</summary>
    private static async Task EnsureMainNavLinkAsync(HoaiiDbContext db, string label, string url, int sortOrder)
    {
        if (await db.NavLinks.AnyAsync(l => l.Placement == NavPlacement.Main && l.Url == url))
        {
            return;
        }

        db.NavLinks.Add(new NavLink
        {
            Placement = NavPlacement.Main,
            Label = label,
            Url = url,
            HasDropdown = false,
            SortOrder = sortOrder,
        });
    }

    /// <summary>Danh mục đứng sau mục menu "Bánh sinh nhật". Chưa có sản phẩm nào nên trang mở ra
    /// là lưới rỗng — vẫn render bình thường, chờ merchandiser thêm hàng trong Admin.</summary>
    private static async Task EnsureBirthdayCakeCategoryAsync(HoaiiDbContext db)
    {
        if (await db.Categories.AnyAsync(c => c.Slug == "banh-sinh-nhat"))
        {
            return;
        }

        db.Categories.Add(new Category
        {
            Name = "Bánh sinh nhật",
            Slug = "banh-sinh-nhat",
            Type = CategoryType.ProductType,
            SortOrder = 10,
            // Banner phẳng như tám trang listing còn lại. Carousel là hero lấy ảnh sản phẩm của
            // chính danh mục, mà danh mục này chưa có sản phẩm nên sẽ ra một hero trống trơn.
            HeroStyle = CategoryHeroStyle.Banner,
            Description = "Bánh sinh nhật đặt riêng cho từng dịp kỷ niệm",
            HeroEyebrow = "Bánh sinh nhật đặc sắc",
        });

        await db.SaveChangesAsync();
    }
}
