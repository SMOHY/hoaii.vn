using Hoaii.Web.Services.Translation;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Hoaii.Web.Controllers;

/// <summary>
/// Đổi ngôn ngữ hiển thị. Chỉ ghi một cookie rồi quay lại trang cũ — không có URL riêng
/// cho từng ngôn ngữ (không /en/...), nên đường dẫn giữ nguyên tiếng Việt ở cả hai chế độ.
/// </summary>
[Route("ngon-ngu")]
public class LanguageController : Controller
{
    [HttpGet("{culture}")]
    public IActionResult Set(string culture, string? returnUrl)
    {
        // Chỉ nhận đúng hai giá trị được phép. Không nhận bừa rồi để CultureInfo tự diễn giải,
        // vì chuỗi culture lạ sẽ thành một cookie vô nghĩa nằm lại trên máy khách.
        var chosen = culture == GeminiTranslator.EnglishCulture
            ? GeminiTranslator.EnglishCulture
            : GeminiTranslator.VietnameseCulture;

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(chosen)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true, // lựa chọn ngôn ngữ là cookie thiết yếu, không cần xin đồng ý
                Path = "/",
                SameSite = SameSiteMode.Lax,
            });

        // IsLocalUrl chặn chuyển hướng ra ngoài: nếu không, ai đó có thể gửi link
        // /ngon-ngu/en?returnUrl=https://trang-gia-mao... và mượn tên miền của mình để lừa khách.
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return Redirect("/");
    }
}
