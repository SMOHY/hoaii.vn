namespace Hoaii.Web.Models;

/// <summary>
/// Chọn màu chữ cho dải campaign dựa trên màu nền của nó.
///
/// Figma ghép sẵn từng cặp: nền vàng đồng #AA8656 đi với chữ trắng, còn nền nhạt #E5D9CB và
/// #E4C0D3 đi với chữ #0F0F0F. Khi màu nền trở thành dữ liệu sửa được trong admin, màu chữ
/// không thể tiếp tục nằm cứng trong CSS — chữ trắng trên nền #E5D9CB chỉ đạt tương phản
/// 1.39:1, tức gần như không đọc được, và nó rơi vào 11 trang.
///
/// Tự suy ra từ độ sáng thay vì thêm một cột nữa: người quản trị đổi màu nền trong admin là chữ
/// tự đổi theo, không ai phải nhớ chỉnh kèm.
/// </summary>
public static class PromoContrast
{
    private const string Dark = "#0F0F0F";  // Foundation/Grey/grey-900
    private const string Light = "#FFFFFF";

    /// <summary>Trả về màu chữ đọc được trên nền đã cho. Nền rỗng/không hợp lệ trả về null để
    /// stylesheet dùng giá trị mặc định của nó.</summary>
    public static string? ForBackground(string? hex)
    {
        var rgb = Parse(hex);
        return rgb is null ? null : Luminance(rgb.Value) > 0.45 ? Dark : Light;
    }

    private static (double R, double G, double B)? Parse(string? hex)
    {
        if (hex is null || hex.Length == 0 || hex[0] != '#')
        {
            return null;
        }

        var body = hex[1..];
        // #abc là dạng viết tắt của #aabbcc.
        if (body.Length == 3)
        {
            body = string.Concat(body.Select(c => new string(c, 2)));
        }

        if (body.Length != 6 || !int.TryParse(body, System.Globalization.NumberStyles.HexNumber,
                System.Globalization.CultureInfo.InvariantCulture, out var v))
        {
            return null;
        }

        return ((v >> 16) & 0xFF, (v >> 8) & 0xFF, v & 0xFF);
    }

    /// <summary>Độ sáng tương đối theo WCAG — cùng công thức dùng để tính tỉ lệ tương phản.</summary>
    private static double Luminance((double R, double G, double B) c)
    {
        static double Channel(double v)
        {
            v /= 255.0;
            return v <= 0.03928 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4);
        }

        return 0.2126 * Channel(c.R) + 0.7152 * Channel(c.G) + 0.0722 * Channel(c.B);
    }
}
