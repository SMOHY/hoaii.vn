using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Hoaii.Web.Areas.Admin.Filters;

/// <summary>
/// Giới hạn dung lượng cho một action nhận tệp tải lên.
///
/// Vì sao cần thứ này thay vì <c>[RequestSizeLimit]</c>: giới hạn thật của một lần tải lên trước
/// đây nằm rải ở ba nơi không ai đồng bộ với ai — trần mặc định của Kestrel (30.000.000 byte),
/// thuộc tính <c>[RequestSizeLimit]</c> đặt tay trên từng action, và hằng số dung lượng trong
/// MediaService mà cả câu chữ trên giao diện lẫn phép kiểm khi lưu đều dựa vào. Hậu quả đo được:
/// giao diện hứa 60MB, nhưng tệp 29MB đã làm trình duyệt treo rồi đứt kết nối mà không một dòng
/// thông báo, vì Kestrel cắt từ tầng dưới, trước khi request kịp vào controller.
///
/// Thuộc tính này gộp cả hai việc vào một con số duy nhất do người gọi truyền vào:
/// nới trần của Kestrel vừa đủ để request vào được, rồi tự kiểm Content-Length và trả về một
/// thông báo tiếng Việt. Người dùng thấy "Tệp vượt quá 60MB" thay vì trang trắng.
/// </summary>
/// <remarks>
/// Phải là <see cref="IAuthorizationFilter"/> với thứ tự chạy sớm nhất, không phải
/// <c>IResourceFilter</c>: <c>[ValidateAntiForgeryToken]</c> cũng là authorization filter và nó
/// ĐỌC thân request để lấy token. Đặt ở tầng resource là quá muộn — thân request đã bị đọc, trần
/// của Kestrel đã chốt, và tệp 29MB vẫn làm đứt kết nối y như trước khi sửa (đã đo).
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public sealed class GioiHanTepAttribute(long soByte, string nhan) : Attribute, IAuthorizationFilter, IOrderedFilter
{
    /// <summary>Phần dôi cho các ô chữ, ranh giới multipart và tiêu đề — bản thân tệp mới là phần lớn.</summary>
    private const long PhanDoi = 2 * 1024 * 1024;

    private long TranThat => soByte + PhanDoi;

    /// <summary>Chạy trước mọi filter khác, kể cả kiểm tra chống giả mạo biểu mẫu.</summary>
    public int Order => int.MinValue;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Nới trần Kestrel TRƯỚC khi thân request được đọc, nếu không nó cắt ở 30MB mặc định.
        var feature = context.HttpContext.Features.Get<IHttpMaxRequestBodySizeFeature>();
        if (feature is { IsReadOnly: false })
        {
            feature.MaxRequestBodySize = TranThat;
        }

        var doDai = context.HttpContext.Request.ContentLength;
        if (doDai is null || doDai <= TranThat)
        {
            return;
        }

        // Quá cỡ: dừng ngay, đừng đọc thân request. Báo bằng đúng con số ghi trên giao diện.
        // Lọc này chạy TRƯỚC khi controller được dựng, nên lấy TempData thẳng từ dịch vụ —
        // chính vì chạy sớm mà nó chặn được trước khi thân request bị đọc.
        var tempData = context.HttpContext.RequestServices
            .GetRequiredService<ITempDataDictionaryFactory>()
            .GetTempData(context.HttpContext);
        tempData["AdminError"] = $"Tệp vượt quá {nhan}. Hãy nén lại hoặc chọn tệp nhỏ hơn.";

        var quayVe = context.HttpContext.Request.Headers.Referer.ToString();
        var noiQuayVe = !string.IsNullOrWhiteSpace(quayVe) && Uri.TryCreate(quayVe, UriKind.Absolute, out var u)
                      && u.Host == context.HttpContext.Request.Host.Host
            ? u.PathAndQuery
            : "/admin";
        context.Result = new RedirectResult(noiQuayVe);
    }
}
