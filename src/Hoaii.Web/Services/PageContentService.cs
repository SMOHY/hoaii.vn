using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Hoaii.Web.Services;

public enum FieldKind { Text, Multiline, Image }

/// <summary>Renders admin-entered multiline text safely: HTML-encode, then turn newlines into
/// &lt;br&gt; and blank lines into paragraph breaks. Never emits raw user HTML.</summary>
public static class ContentText
{
    public static Microsoft.AspNetCore.Html.IHtmlContent Lines(string? s) =>
        new Microsoft.AspNetCore.Html.HtmlString(
            System.Net.WebUtility.HtmlEncode(s ?? "").Replace("\r\n", "\n").Replace("\n", "<br />"));
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

    public static readonly IReadOnlyList<Field> All =
    [
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
