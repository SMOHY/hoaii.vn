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
        if (!await db.NavLinks.AnyAsync())
        {
            db.NavLinks.AddRange(
                new NavLink { Placement = NavPlacement.Main, Label = "Quà tết", Url = "/danh-muc/qua-tet", HasDropdown = true, SortOrder = 0 },
                new NavLink { Placement = NavPlacement.Main, Label = "Quà trung thu", Url = "/danh-muc/qua-trung-thu", HasDropdown = true, SortOrder = 1 },
                new NavLink { Placement = NavPlacement.Main, Label = "Quà theo dịp", Url = "/qua-theo-dip", HasDropdown = true, SortOrder = 2 },
                new NavLink { Placement = NavPlacement.Main, Label = "Sản phẩm chọn lọc", Url = "/danh-muc/san-pham-chon-loc", HasDropdown = true, SortOrder = 3 },
                new NavLink { Placement = NavPlacement.Sub, Label = "Về chúng tôi", Url = "/ve-chung-toi", SortOrder = 0 },
                new NavLink { Placement = NavPlacement.Sub, Label = "Liên hệ", Url = "/lien-he", SortOrder = 1 },
                new NavLink { Placement = NavPlacement.Sub, Label = "Đại lý", Url = "/hop-tac", SortOrder = 2 },
                new NavLink { Placement = NavPlacement.Sub, Label = "Blog", Url = "/blog", SortOrder = 3 });
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
}
