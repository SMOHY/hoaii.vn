using System.Text.RegularExpressions;

namespace Hoaii.Web.Services;

/// <summary>
/// Link video do admin dán vào. Admin dán đường dẫn trên thanh địa chỉ ("watch?v=..."), còn trang
/// web cần dạng nhúng ("embed/..."), nên việc đổi dạng làm ở đây thay vì bắt admin hiểu khái niệm
/// nhúng. Chỉ nhận YouTube và Vimeo: mọi thứ khác bị từ chối, để không ai dán được một đường dẫn
/// tuỳ ý vào thẻ iframe trên trang bán hàng.
/// </summary>
public static class VideoLink
{
    /// <summary>
    /// Nhận mọi dạng địa chỉ YouTube mà người dùng thật sự copy được: bản máy tính
    /// (<c>www.youtube.com/watch?v=</c>), bản điện thoại (<c>m.youtube.com</c>), bản rút gọn
    /// (<c>youtu.be</c>), YouTube Music, Shorts, phát trực tiếp và dạng nhúng sẵn.
    ///
    /// Thiếu đúng một chữ "m." từng làm mất video của cả một sản phẩm: link copy từ điện thoại
    /// không khớp, hệ thống coi như bỏ trống rồi ghi rỗng đè lên link đang chạy.
    /// </summary>
    private static readonly Regex YouTube = new(
        @"^https?://(?:(?:www|m|music)\.)?(?:youtube(?:-nocookie)?\.com/(?:watch\?(?:.*&)?v=|embed/|shorts/|live/|v/)|youtu\.be/)([A-Za-z0-9_-]{6,20})",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex Vimeo = new(
        @"^https?://(?:(?:www|player)\.)?vimeo\.com/(?:video/)?(\d{6,15})",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>Kết quả đọc ô nhập link — ba trạng thái, không phải hai.</summary>
    public enum KetQua
    {
        /// <summary>Ô để trống: người dùng muốn bỏ video.</summary>
        BoTrong,

        /// <summary>Nhận ra được, dùng luôn.</summary>
        HopLe,

        /// <summary>Có gõ nhưng không nhận ra: KHÔNG được coi là bỏ trống.</summary>
        KhongNhanRa,
    }

    /// <summary>
    /// Đọc ô nhập link. Tách bạch "để trống" với "gõ sai" là điểm mấu chốt: gộp hai trạng thái
    /// này lại chính là thứ đã âm thầm xoá mất video của sản phẩm — admin dán một link không khớp,
    /// hàm trả về null, chỗ gọi ghi null vào CSDL và màn hình vẫn báo "Đã lưu sản phẩm".
    /// </summary>
    public static KetQua Doc(string? nhapVao, out string? link)
    {
        link = null;
        if (string.IsNullOrWhiteSpace(nhapVao))
        {
            return KetQua.BoTrong;
        }

        var goc = nhapVao.Trim();
        if (!YouTube.IsMatch(goc) && !Vimeo.IsMatch(goc))
        {
            return KetQua.KhongNhanRa;
        }

        link = goc;
        return KetQua.HopLe;
    }

    /// <summary>Đường dẫn nhúng tương ứng, hoặc null nếu link không nhận ra được.</summary>
    public static string? Embed(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }
        var goc = url.Trim();

        var yt = YouTube.Match(goc);
        if (yt.Success)
        {
            return $"https://www.youtube-nocookie.com/embed/{yt.Groups[1].Value}";
        }

        var vm = Vimeo.Match(goc);
        return vm.Success ? $"https://player.vimeo.com/video/{vm.Groups[1].Value}" : null;
    }
}
