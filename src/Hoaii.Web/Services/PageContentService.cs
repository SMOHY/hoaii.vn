using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Hoaii.Web.Services;

public enum FieldKind { Text, Multiline, Image }

/// <summary>Renders admin-entered multiline text safely: escape the HTML-significant characters,
/// then turn newlines into &lt;br&gt;. Never emits raw user HTML. Unicode is left alone so Vietnamese
/// stays readable in the source (WebUtility.HtmlEncode would turn it into numeric entities).</summary>
public static class ContentText
{
    public static Microsoft.AspNetCore.Html.IHtmlContent Lines(string? s)
    {
        var html = (s ?? "")
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;")
            .Replace("\r\n", "\n")
            .Replace("\n", "<br />");
        return new Microsoft.AspNetCore.Html.HtmlString(html);
    }
}

/// <summary>
/// Well-known page-content fields with their coded defaults (transcribed from the original static
/// views), so the storefront renders identically before anyone edits and a fresh DB needs no seed.
/// </summary>
public static class PageContentKeys
{
    public record Field(string Page, string Key, string Label, string Default, FieldKind Kind);

    public const string About = "about";
    public const string Partners = "partners";
    public const string Home = "home";
    public const string Footer = "footer";
    public const string Contact = "contact";
    public const string Shop = "shop";

    public static readonly IReadOnlyList<Field> All =
    [
        // ---------- Nội dung dùng chung ở trang bán hàng / blog ----------
        new(Shop, "blog_page_heading", "Trang Blog — tiêu đề", "HOÀI MÁCH BẠN", FieldKind.Text),
        new(Shop, "pdp_price_note", "Trang sản phẩm — ghi chú giá", "(Giá chưa bao gồm 8% VAT)", FieldKind.Text),
        new(Shop, "pdp_related_heading", "Trang sản phẩm — tiêu đề sản phẩm liên quan", "SẢN PHẨM LIÊN QUAN", FieldKind.Text),
        new(Shop, "category_empty_title", "Danh mục trống — tiêu đề", "SẢN PHẨM ĐANG ĐƯỢC CẬP NHẬT", FieldKind.Text),
        new(Shop, "category_empty_text", "Danh mục trống — mô tả (theo sau là link Trang chủ)",
            "Bạn có thể xem sản phẩm khác hoặc quay lại", FieldKind.Text),

        // ---------- Trang chủ: chữ khung của từng mục ----------
        new(Home, "featured_title", "Sản phẩm nổi bật — tiêu đề", "LỰA CHỌN HÀNG ĐẦU", FieldKind.Text),
        new(Home, "featured_subtitle", "Sản phẩm nổi bật — mô tả",
            "Mời bạn thưởng lãm những lựa chọn hàng đầu từ bộ sưu tập quà tặng thiết kế mới nhất. Tinh tế trong từng đường nét, độc bản trong từng câu chuyện.", FieldKind.Multiline),
        new(Home, "featured_card_eyebrow_desktop", "Nhãn thẻ (desktop)", "Phẩm quà", FieldKind.Text),
        new(Home, "featured_card_eyebrow_mobile", "Nhãn thẻ (mobile)", "Bộ sưu tập", FieldKind.Text),
        new(Home, "featured_card_cta_desktop", "Nút thẻ (desktop)", "Khám phá", FieldKind.Text),
        new(Home, "featured_card_cta_mobile", "Nút thẻ (mobile)", "Xem thêm", FieldKind.Text),
        new(Home, "services_eyebrow", "Dịch vụ — eyebrow", "Dịch vụ theo yêu cầu", FieldKind.Text),
        new(Home, "services_title", "Dịch vụ — tiêu đề (xuống dòng = ngắt dòng)",
            "CÁ NHÂN HÓA SẢN PHẨM\nĐỂ MANG DẤU ẤN CỦA RIÊNG BẠN", FieldKind.Multiline),
        new(Home, "story_banner_desktop", "Banner câu chuyện (desktop)", "CÂU CHUYỆN TRONG TỪNG SẢN PHẨM", FieldKind.Text),
        new(Home, "story_banner_mobile_1", "Banner câu chuyện (mobile — dòng 1)", "CÂU CHUYỆN", FieldKind.Text),
        new(Home, "story_banner_mobile_2", "Banner câu chuyện (mobile — dòng 2)", "TRONG TỪNG SẢN PHẨM", FieldKind.Text),
        new(Home, "about_heading", "Về Hoài — tiêu đề", "VỀ HOÀI", FieldKind.Text),
        new(Home, "about_subtitle", "Về Hoài — mô tả",
            "Khởi nguồn từ tình yêu dành cho di sản Việt Nam, HOÀI chọn hành trình gìn giữ và lan tỏa giá trị truyền thống thông qua những tặng phẩm văn hóa cao cấp. Bằng lăng kính của thế hệ trẻ, chúng tôi đưa nét đẹp xưa hòa cùng nhịp sống hôm nay, để ký ức, câu chuyện và tinh thần dân tộc được tiếp nối một cách mới mẻ, đầy cảm hứng qua ngôn ngữ thiết kế đương đại.", FieldKind.Multiline),
        new(Home, "customers_heading", "Khách hàng — tiêu đề", "KHÁCH HÀNG CỦA CHÚNG TÔI", FieldKind.Text),
        new(Home, "blog_heading_desktop", "Blog — tiêu đề (desktop)", "HOÀI KỂ BẠN NGHE", FieldKind.Text),
        new(Home, "blog_heading_mobile", "Blog — tiêu đề (mobile)", "HOÀI MÁCH BẠN", FieldKind.Text),

        // ---------- Chân trang: khối đăng ký nhận tin ----------
        new(Footer, "newsletter_heading", "Tiêu đề", "ĐĂNG KÝ VÀO DANH SÁCH", FieldKind.Text),
        new(Footer, "newsletter_text", "Mô tả",
            "Cập nhật sớm nhất thông tin sản phẩm mới và những đặc quyền giới hạn từ HOÀI!", FieldKind.Multiline),
        new(Footer, "newsletter_placeholder", "Gợi ý ô nhập email", "Nhập email của bạn", FieldKind.Text),
        new(Footer, "newsletter_button", "Chữ nút", "Gửi", FieldKind.Text),
        new(Footer, "newsletter_thanks", "Lời cảm ơn sau khi đăng ký",
            "Cảm ơn bạn! Chúng tôi sẽ gửi tin mới nhất tới hộp thư của bạn.", FieldKind.Multiline),

        // ---------- Trang Liên hệ ----------
        new(Contact, "hero_title", "Tiêu đề hero", "LIÊN HỆ", FieldKind.Text),
        new(Contact, "hero_image", "Ảnh hero", "/images/contact/hero.jpg", FieldKind.Image),
        new(Contact, "hero_caption", "Chú thích hero", "Hãy liên hệ với chúng tôi", FieldKind.Text),
        new(Contact, "address_heading", "Tiêu đề mục địa chỉ", "Địa chỉ", FieldKind.Text),
        new(Contact, "map_url", "Link bản đồ", "https://maps.google.com/?q=945+Ngô+Gia+Tự,+Việt+Hưng,+Hà+Nội", FieldKind.Text),
        new(Contact, "map_image", "Ảnh bản đồ", "/images/contact/map.jpg", FieldKind.Image),
        new(Contact, "map_card_image", "Ảnh thẻ địa điểm", "/images/contact/map-card.png", FieldKind.Image),
        new(Contact, "methods_heading", "Tiêu đề mục liên hệ", "Liên hệ", FieldKind.Text),
        new(Contact, "method_1_title", "Thẻ 1 — tiêu đề", "TRÒ CHUYỆN TRỰC TIẾP", FieldKind.Text),
        new(Contact, "method_1_text", "Thẻ 1 — mô tả",
            "Hãy truy cập facebook hoặc zalo của chúng tôi để nói chuyện trực tiếp với đội ngũ chăm sóc khách hàng về bất kỳ thắc mắc nào.", FieldKind.Multiline),
        new(Contact, "method_1_link_label", "Thẻ 1 — chữ liên kết", "Liên hệ facebook", FieldKind.Text),
        new(Contact, "method_2_title", "Thẻ 2 — tiêu đề", "LIÊN QUAN ĐẾN ĐƠN HÀNG", FieldKind.Text),
        new(Contact, "method_2_text", "Thẻ 2 — mô tả",
            "Hãy truy cập facebook hoặc zalo của chúng tôi để nói chuyện trực tiếp với đội ngũ chăm sóc khách hàng về bất kỳ thắc mắc nào.", FieldKind.Multiline),
        new(Contact, "method_2_link_label", "Thẻ 2 — chữ nút", "Trò chuyện trực tiếp", FieldKind.Text),
        new(Contact, "method_3_title", "Thẻ 3 — tiêu đề", "YÊU CẦU BÁN BUÔN", FieldKind.Text),
        new(Contact, "method_3_text", "Thẻ 3 — mô tả", "Bạn muốn xem mẫu? Vui lòng gửi email cho chúng tôi", FieldKind.Multiline),
        new(Contact, "method_4_title", "Thẻ 4 — tiêu đề", "ĐIỆN THOẠI", FieldKind.Text),
        new(Contact, "method_4_text", "Thẻ 4 — mô tả", "Hãy gửi cho chúng tôi một lời nhắn", FieldKind.Multiline),
        new(Contact, "form_heading", "Tiêu đề form gửi email", "Hãy gửi email cho chúng tôi", FieldKind.Text),
        new(Contact, "form_intro", "Lời dẫn cạnh form",
            "Chúng tôi rất vui vì điều gì đó ở đây đã thu hút sự chú ý của bạn. Hãy liên hệ để chào hỏi, trao đổi về nhu cầu của bạn và cùng nhau tìm hiểu cách hợp tác nhé.", FieldKind.Multiline),
        new(Contact, "form_success", "Lời cảm ơn sau khi gửi",
            "Cảm ơn bạn đã liên hệ! Chúng tôi sẽ phản hồi sớm nhất có thể.", FieldKind.Multiline),

        // ---------- Về chúng tôi ----------
        new(About, "hero_headline_desktop", "Tiêu đề hero (desktop)",
            "HOÀI - CHÚNG TÔI LÀ NHỮNG NGƯỜI TRẺ, BƯỚC ĐI VỚI MONG MUỐN ĐEM CÂU CHUYỆN VĂN HÓA DÂN TỘC LAN TỎA KHẮP MUÔN NƠI.", FieldKind.Multiline),
        new(About, "hero_headline_mobile", "Tiêu đề hero (mobile)",
            "CHÚNG TÔI LÀ MỘT NHÓM THIẾT KẾ ĐỘC LẬP, ĐƯỢC THÀNH LẬP VÀO NĂM 2021 VÀ CÓ VĂN PHÒNG TẠI HÀ NỘI", FieldKind.Multiline),
        new(About, "hero_image", "Ảnh hero", "/images/about/hero.jpg", FieldKind.Image),
        new(About, "caption_label", "Nhãn chú thích", "KHỞI ĐẦU", FieldKind.Text),
        new(About, "caption_1", "Chú thích — đoạn 1",
            "Khởi nguồn từ tình yêu dành cho di sản Việt Nam, HOÀI chọn hành trình gìn giữ và lan tỏa giá trị truyền thống thông qua những tặng phẩm văn hóa cao cấp.", FieldKind.Multiline),
        new(About, "caption_2", "Chú thích — đoạn 2",
            "Mỗi sản phẩm tại HOÀI đều được chăm chút tỉ mỉ như một lát cắt văn hóa tinh tế, nơi mỹ cảm thời đại song hành cùng chiều sâu nội dung. Không đơn thuần là thức quà mang dấu ấn bản địa, đây còn là nơi chắt lọc tinh hoa từ phong tục và nếp sống, trở thành cầu nối ngoại giao tinh tế giúp các cá nhân, tổ chức kết nối thâm giao.", FieldKind.Multiline),
        new(About, "quote", "Câu trích dẫn", "Gói ghém chân tình, viết tiếp dòng di sản", FieldKind.Text),
        new(About, "story", "Câu chuyện (đoạn trống = xuống dòng)",
            "Văn hóa là một dòng chảy bất tận. Tại HOÀI, chúng tôi chọn đứng nơi giao lộ của thời gian để thực hiện một nhiệm vụ đơn giản nhưng bền bỉ: Gói ghém chân tình, viết tiếp dòng di sản.\n\nBằng lăng kính của thế hệ trẻ, chúng tôi đưa nét đẹp xưa hòa cùng nhịp sống hôm nay, để ký ức, câu chuyện và tinh thần dân tộc được tiếp nối một cách mới mẻ, đầy cảm hứng qua ngôn ngữ thiết kế đương đại.", FieldKind.Multiline),
        new(About, "story_image", "Ảnh câu chuyện", "/images/about/story.jpg", FieldKind.Image),
        new(About, "foundation_heading", "Tiêu đề Nền tảng", "NỀN TẢNG THƯƠNG HIỆU", FieldKind.Text),
        new(About, "found_1_title", "Nền tảng 01 — tiêu đề", "Mục đích", FieldKind.Text),
        new(About, "found_1_body", "Nền tảng 01 — nội dung",
            "HOÀI ra đời từ niềm tin: Di sản chỉ thực sự sống khi được tiếp nối. Không nhìn truyền thống như một khái niệm đứng yên, chúng tôi chọn tinh thần “Gìn giữ để Khởi sinh” làm kim chỉ nam để tái hiện những giá trị xưa cũ qua cảm quan nghệ thuật đương đại.\n\nNhững món quà, vật phẩm từ HOÀI là sự giao thoa hoàn mỹ giữa kỹ nghệ tinh xảo và bề dày văn hóa, biến mỗi vật phẩm trao đi trở thành một câu chuyện di sản được viết tiếp đầy kiêu hãnh.", FieldKind.Multiline),
        new(About, "found_2_title", "Nền tảng 02 — tiêu đề", "Tầm nhìn", FieldKind.Text),
        new(About, "found_2_body", "Nền tảng 02 — nội dung",
            "Trở thành biểu tượng của ngành quà tặng thiết kế cao cấp tại Việt Nam, là thương hiệu đầu tiên khách hàng nghĩ đến khi muốn tặng một món quà mang “quốc hồn quốc túy” nhưng có thẩm mỹ đương đại.", FieldKind.Multiline),
        new(About, "found_3_title", "Nền tảng 03 — tiêu đề", "Sứ mệnh", FieldKind.Text),
        new(About, "found_3_body", "Nền tảng 03 — nội dung",
            "Sứ mệnh của Hoài là đánh thức những giá trị di sản thông qua hơi thở thiết kế đương đại, biến mỗi món quà trở thành một sứ giả văn hoá kết nối quá khứ với hiện tại. Chúng tôi chăm chút trong từng bao bì và vật phẩm tinh xảo để người Việt tìm thấy niềm tự hào trong gốc rễ, và để bạn bè năm châu chạm đến những tầng sâu tinh tế của tâm hồn Việt giữa dòng chảy thời đại.", FieldKind.Multiline),
        new(About, "team_heading", "Tiêu đề Đội ngũ", "ĐỘI NGŨ", FieldKind.Text),
        new(About, "team_subtitle", "Mô tả Đội ngũ",
            "Chúng tôi là những người trẻ cùng chung tình yêu dành cho văn hóa và thủ công Việt. Bằng sự thấu hiểu, tinh thần sáng tạo và sự chỉn chu trong từng chi tiết, đội ngũ HOÀI không ngừng kết nối giá trị truyền thống với nhịp sống đương đại, để mỗi sản phẩm trở thành một món quà mang theo câu chuyện, cảm xúc và dấu ấn riêng.", FieldKind.Multiline),
        new(About, "team_img_wide", "Ảnh đội ngũ — rộng", "/images/about/team-wide.jpg", FieldKind.Image),
        new(About, "team_img_small_1", "Ảnh đội ngũ — nhỏ 1", "/images/about/team-small-1.jpg", FieldKind.Image),
        new(About, "team_img_small_2", "Ảnh đội ngũ — nhỏ 2", "/images/about/team-small-2.jpg", FieldKind.Image),
        new(About, "team_img_main", "Ảnh đội ngũ — chính", "/images/about/team-main.jpg", FieldKind.Image),

        // ---------- Hợp tác ----------
        new(Partners, "stats_heading", "Tiêu đề số liệu", "Thương hiệu Hoài tự hào với", FieldKind.Text),
        new(Partners, "stat_1_number", "Số liệu 1 — số", "6+", FieldKind.Text),
        new(Partners, "stat_1_label", "Số liệu 1 — nhãn", "Năm thành lập và phát triển", FieldKind.Text),
        new(Partners, "stat_2_number", "Số liệu 2 — số", "100+", FieldKind.Text),
        new(Partners, "stat_2_label", "Số liệu 2 — nhãn", "Đối tác tập đoàn hàng đầu Việt Nam", FieldKind.Text),
        new(Partners, "stat_3_number", "Số liệu 3 — số", "150.000+", FieldKind.Text),
        new(Partners, "stat_3_label", "Số liệu 3 — nhãn", "Sản phẩm đã được gửi trao", FieldKind.Text),
        new(Partners, "wholesale_heading", "Tiêu đề mục mua sỉ", "Yêu cầu mua sỉ", FieldKind.Text),
        new(Partners, "wholesale_image", "Ảnh mục mua sỉ", "/images/partners/wholesale.jpg", FieldKind.Image),
    ];

    public static IReadOnlyList<Field> ForPage(string page) => All.Where(f => f.Page == page).ToList();
}

/// <summary>Reads/writes page content, caching the whole set (read on every About/Partners render).</summary>
public class PageContentService(HoaiiDbContext db, IMemoryCache cache)
{
    private const string CacheKey = "page_content_all";

    private Dictionary<string, string> Load() =>
        cache.GetOrCreate(CacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            return db.PageContents.AsNoTracking().ToDictionary(c => c.PageKey + "|" + c.BlockKey, c => c.Value);
        })!;

    /// <summary>Value for a page+block, falling back to the coded default (never null).</summary>
    public string Get(string page, string key)
    {
        if (Load().TryGetValue(page + "|" + key, out var v) && !string.IsNullOrWhiteSpace(v))
        {
            return v;
        }
        return PageContentKeys.All.FirstOrDefault(f => f.Page == page && f.Key == key)?.Default ?? "";
    }

    public IReadOnlyDictionary<string, string> GetForEditing(string page)
    {
        var stored = Load();
        return PageContentKeys.ForPage(page).ToDictionary(
            f => f.Key,
            f => stored.TryGetValue(page + "|" + f.Key, out var v) ? v : f.Default);
    }

    public async Task SaveAsync(string page, IDictionary<string, string?> values)
    {
        var known = PageContentKeys.ForPage(page).Select(f => f.Key).ToHashSet();
        var existing = await db.PageContents.Where(c => c.PageKey == page).ToDictionaryAsync(c => c.BlockKey);
        foreach (var (key, value) in values)
        {
            if (!known.Contains(key)) continue;
            var v = value?.Trim() ?? "";
            if (existing.TryGetValue(key, out var row)) row.Value = v;
            else db.PageContents.Add(new PageContent { PageKey = page, BlockKey = key, Value = v });
        }
        await db.SaveChangesAsync();
        cache.Remove(CacheKey);
    }
}
