namespace Hoaii.Web.Models.Layout;

/// <summary>
/// Static placeholder content for Nav/Footer until these are backed by
/// category data and a CMS-managed footer configuration.
/// </summary>
public static class LayoutData
{
    public static IReadOnlyList<NavMenuItem> MainMenu { get; } =
    [
        new() { Label = "Quà tết", Url = "/danh-muc/qua-tet", HasDropdown = true },
        new() { Label = "Quà trung thu", Url = "/danh-muc/qua-trung-thu", HasDropdown = true },
        new() { Label = "Quà theo dịp", Url = "/qua-theo-dip", HasDropdown = true },
        new() { Label = "Sản phẩm chọn lọc", Url = "/danh-muc/san-pham-chon-loc", HasDropdown = true },
    ];

    public static IReadOnlyList<NavMenuItem> SubNavLinks { get; } =
    [
        new() { Label = "Về chúng tôi", Url = "/ve-chung-toi" },
        new() { Label = "Liên hệ", Url = "/lien-he" },
        new() { Label = "Đại lý", Url = "/hop-tac" },
        new() { Label = "Blog", Url = "/blog" },
    ];

    public static FooterViewModel Footer { get; } = new()
    {
        Columns =
        [
            new FooterColumn
            {
                Title = "VỀ HOÀI",
                Links =
                [
                    new() { Label = "Quà tết", Url = "/danh-muc/qua-tet" },
                    new() { Label = "Quà trung thu", Url = "/danh-muc/qua-trung-thu" },
                    new() { Label = "Quà theo dịp", Url = "/qua-theo-dip" },
                    new() { Label = "Sản phẩm chọn lọc", Url = "/danh-muc/san-pham-chon-loc" },
                    new() { Label = "Câu chuyện", Url = "/blog" },
                    new() { Label = "Đối tác", Url = "/hop-tac" },
                ],
            },
            new FooterColumn
            {
                Title = "HỖ TRỢ KHÁCH HÀNG",
                Links =
                [
                    new() { Label = "Liên hệ", Url = "/lien-he" },
                    new() { Label = "Chính sách trao đổi & hoàn tác", Url = "/chinh-sach/trao-doi" },
                    new() { Label = "Chính sách giao nhận hàng hóa", Url = "/chinh-sach/giao-hang" },
                ],
            },
            new FooterColumn
            {
                Title = "CHÍNH SÁCH PHÁP LÝ",
                Links =
                [
                    new() { Label = "Điều khoản sử dụng", Url = "/chinh-sach/dieu-khoan-su-dung" },
                    new() { Label = "Chính sách bảo vệ dữ liệu cá nhân", Url = "/chinh-sach/bao-mat" },
                ],
            },
        ],
        SocialLinks =
        [
            // Figma's footer shows Facebook / Instagram / TikTok, not Zalo (node 683:4422).
            new() { Name = "Facebook", Url = "https://facebook.com", IconPath = "/images/icons/social-facebook.svg" },
            new() { Name = "Instagram", Url = "https://instagram.com", IconPath = "/images/icons/social-instagram.svg" },
            new() { Name = "TikTok", Url = "https://tiktok.com", IconPath = "/images/icons/social-tiktok.svg" },
        ],
    };
}
