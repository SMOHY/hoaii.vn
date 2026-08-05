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
                    ],
                },
                new FooterMenuColumn
                {
                    // Điều 17.1.a Nghị định 248/2026: every required-disclosure page must be
                    // grouped into one clearly-labelled block, distinct from support/business
                    // links — not split across "HỖ TRỢ KHÁCH HÀNG" and a generic "pháp lý" column
                    // the way it was before.
                    Title = "CHÍNH SÁCH & ĐIỀU KHOẢN", SortOrder = 2,
                    Links = PolicyFooterLinks(),
                });
        }
        else
        {
            await EnsurePolicyColumnAsync(db);
        }

        await db.SaveChangesAsync();
    }

    private static List<FooterMenuLink> PolicyFooterLinks() =>
    [
        new() { Label = "Thông tin chủ sở hữu", Url = "/chinh-sach/thong-tin-chu-so-huu", SortOrder = 0 },
        new() { Label = "Chính sách bảo vệ dữ liệu cá nhân", Url = "/chinh-sach/bao-mat", SortOrder = 1 },
        new() { Label = "Quyền và nghĩa vụ các bên", Url = "/chinh-sach/quyen-va-nghia-vu", SortOrder = 2 },
        new() { Label = "Giải quyết khiếu nại", Url = "/chinh-sach/khieu-nai", SortOrder = 3 },
        new() { Label = "Chính sách giá & thanh toán", Url = "/chinh-sach/gia-thanh-toan", SortOrder = 4 },
        new() { Label = "Chính sách ưu tiên hiển thị", Url = "/chinh-sach/uu-tien-hien-thi", SortOrder = 5 },
        new() { Label = "Điều kiện, hạn chế cung cấp hàng hóa", Url = "/chinh-sach/dieu-kien-han-che", SortOrder = 6 },
        new() { Label = "Chính sách giao nhận hàng hóa", Url = "/chinh-sach/giao-hang", SortOrder = 7 },
        new() { Label = "Chính sách trao đổi & hoàn tác", Url = "/chinh-sach/trao-doi", SortOrder = 8 },
        new() { Label = "Điều khoản sử dụng", Url = "/chinh-sach/dieu-khoan-su-dung", SortOrder = 9 },
    ];

    /// <summary>
    /// Migrates a footer that was already seeded under the old layout (policy links split
    /// between "HỖ TRỢ KHÁCH HÀNG" and "CHÍNH SÁCH PHÁP LÝ") to the single grouped column Điều
    /// 17.1.a requires. Renames "CHÍNH SÁCH PHÁP LÝ" → "CHÍNH SÁCH & ĐIỀU KHOẢN" if found (keeps
    /// its Id, so it stays the "last column" the social/badge block attaches to), strips the
    /// policy links that had leaked into "HỖ TRỢ KHÁCH HÀNG", and adds any policy link still
    /// missing. Idempotent — every step checks before acting, safe to run on every startup.
    /// </summary>
    private static async Task EnsurePolicyColumnAsync(HoaiiDbContext db)
    {
        var legacyPolicyColumn = await db.FooterMenuColumns
            .Include(c => c.Links)
            .FirstOrDefaultAsync(c => c.Title == "CHÍNH SÁCH PHÁP LÝ");
        var policyColumn = legacyPolicyColumn
            ?? await db.FooterMenuColumns.Include(c => c.Links).FirstOrDefaultAsync(c => c.Title == "CHÍNH SÁCH & ĐIỀU KHOẢN");
        if (policyColumn is null)
        {
            return; // Fresh-seed branch above already created it.
        }
        policyColumn.Title = "CHÍNH SÁCH & ĐIỀU KHOẢN";

        var support = await db.FooterMenuColumns
            .Include(c => c.Links)
            .FirstOrDefaultAsync(c => c.Title == "HỖ TRỢ KHÁCH HÀNG");
        if (support is not null)
        {
            var strayPolicyLinks = support.Links.Where(l => l.Url.StartsWith("/chinh-sach/")).ToList();
            foreach (var link in strayPolicyLinks)
            {
                support.Links.Remove(link);
                db.FooterMenuLinks.Remove(link);
            }
        }

        foreach (var wanted in PolicyFooterLinks())
        {
            if (!policyColumn.Links.Any(l => l.Url == wanted.Url))
            {
                policyColumn.Links.Add(new FooterMenuLink { Label = wanted.Label, Url = wanted.Url, SortOrder = wanted.SortOrder });
            }
        }
    }
}
