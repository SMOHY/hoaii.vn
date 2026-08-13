using Hoaii.Domain.Entities;

namespace Hoaii.Web.Services;

/// <summary>
/// HTML for the two order-related transactional emails (customer confirmation + HOÀI-internal
/// new-order notification). Table-based layout with inline styles only — email clients (Outlook
/// desktop especially) strip &lt;style&gt; blocks and ignore most flexbox/grid CSS, so this can't
/// reuse the site's own stylesheet the way every other page does.
/// </summary>
public static class OrderEmailTemplates
{
    private const string Red = "#AF2234";
    private const string RedDeep = "#410C13";
    private const string Cream = "#F7F3EE";
    private const string GreyText = "#6B6B6B";
    private const string Border = "#E5E1DB";

    private static string Enc(string? s) => System.Net.WebUtility.HtmlEncode(s ?? "");

    private static string Shell(string preheader, string bodyHtml) => $"""
        <div style="display:none;max-height:0;overflow:hidden;opacity:0;">{Enc(preheader)}</div>
        <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background:#F0EBE1;padding:32px 16px;font-family:Arial,Helvetica,sans-serif;">
          <tr><td align="center">
            <table role="presentation" width="600" cellpadding="0" cellspacing="0" style="max-width:600px;width:100%;background:#ffffff;border-radius:10px;overflow:hidden;">
              <tr>
                <td style="background:{RedDeep};padding:26px 32px;text-align:center;">
                  <span style="font-family:Georgia,'Times New Roman',serif;font-size:24px;letter-spacing:6px;color:#ffffff;">HOÀI</span>
                </td>
              </tr>
              <tr><td style="padding:32px;color:#2B2B2B;font-size:15px;line-height:1.6;">
                {bodyHtml}
              </td></tr>
              <tr>
                <td style="background:{Cream};padding:18px 32px;text-align:center;font-size:12px;color:{GreyText};">
                  HOÀI — Quà tặng văn hóa Việt<br/>
                  Mọi thắc mắc vui lòng trả lời email này hoặc liên hệ hotline.
                </td>
              </tr>
            </table>
          </td></tr>
        </table>
        """;

    private static string ItemsTable(IEnumerable<OrderItem> items) => $"""
        <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="margin:18px 0;font-size:14px;">
          {string.Join("", items.Select(i => $"""
          <tr>
            <td style="padding:10px 0;border-bottom:1px solid {Border};">
              {Enc(i.ProductName)}{(string.IsNullOrEmpty(i.VariantName) ? "" : $"<br/><span style=\"color:{GreyText};font-size:13px;\">{Enc(i.VariantName)}</span>")}
            </td>
            <td style="padding:10px 0;border-bottom:1px solid {Border};text-align:center;color:{GreyText};white-space:nowrap;">×{i.Quantity}</td>
            <td style="padding:10px 0;border-bottom:1px solid {Border};text-align:right;white-space:nowrap;">{i.UnitPrice * i.Quantity:N0}đ</td>
          </tr>
          """))}
        </table>
        """;

    public static (string Subject, string Html) CustomerConfirmation(Order order)
    {
        var body = $"""
            <h1 style="margin:0 0 6px;font-family:Georgia,'Times New Roman',serif;font-weight:normal;font-size:22px;color:{RedDeep};">Cảm ơn bạn đã đặt hàng!</h1>
            <p style="margin:0 0 20px;color:{GreyText};">Đơn hàng của bạn tại HOÀI đã được ghi nhận. Chúng tôi sẽ liên hệ để xác nhận và giao hàng sớm nhất.</p>
            <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background:{Cream};border-radius:6px;">
              <tr><td style="padding:14px 18px;font-size:14px;">Mã đơn hàng&ensp;<strong style="color:{Red};">{Enc(order.OrderNumber)}</strong></td></tr>
            </table>
            {ItemsTable(order.Items)}
            <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="font-size:14px;">
              <tr><td style="padding:4px 0;color:{GreyText};">Tạm tính</td><td style="padding:4px 0;text-align:right;">{order.Subtotal:N0}đ</td></tr>
              {(order.Discount > 0 ? $"""<tr><td style="padding:4px 0;color:{GreyText};">Giảm giá</td><td style="padding:4px 0;text-align:right;">-{order.Discount:N0}đ</td></tr>""" : "")}
              <tr><td style="padding:4px 0;color:{GreyText};">Vận chuyển</td><td style="padding:4px 0;text-align:right;">{(order.ShippingFee > 0 ? $"{order.ShippingFee:N0}đ" : "Miễn phí")}</td></tr>
              <tr><td style="padding:14px 0 0;font-weight:bold;font-size:18px;border-top:1px solid {Border};">Tổng cộng</td><td style="padding:14px 0 0;font-weight:bold;font-size:18px;text-align:right;color:{Red};border-top:1px solid {Border};">{order.Total:N0}đ</td></tr>
            </table>
            """;
        return ($"Xác nhận đơn hàng {order.OrderNumber} — HOÀI", Shell($"Cảm ơn bạn đã đặt hàng — mã đơn {order.OrderNumber}", body));
    }

    public static (string Subject, string Html) InternalNotification(Order order, string adminUrl)
    {
        var infoRows = $"""
            <tr><td style="padding:4px 14px 4px 0;color:{GreyText};white-space:nowrap;">Khách hàng</td><td style="padding:4px 0;font-weight:bold;">{Enc($"{order.FirstName} {order.LastName}")}</td></tr>
            <tr><td style="padding:4px 14px 4px 0;color:{GreyText};">SĐT</td><td style="padding:4px 0;">{Enc(order.Phone)}</td></tr>
            <tr><td style="padding:4px 14px 4px 0;color:{GreyText};">Email</td><td style="padding:4px 0;">{Enc(order.Email)}</td></tr>
            <tr><td style="padding:4px 14px 4px 0;color:{GreyText};vertical-align:top;">Địa chỉ</td><td style="padding:4px 0;">{Enc(order.Address)}, {Enc(order.ProvinceDistrictWard)}</td></tr>
            {(string.IsNullOrWhiteSpace(order.Notes) ? "" : $"""<tr><td style="padding:4px 14px 4px 0;color:{GreyText};vertical-align:top;">Ghi chú</td><td style="padding:4px 0;">{Enc(order.Notes)}</td></tr>""")}
            <tr><td style="padding:4px 14px 4px 0;color:{GreyText};">Thanh toán</td><td style="padding:4px 0;">{order.PaymentMethod}</td></tr>
            <tr><td style="padding:4px 14px 4px 0;color:{GreyText};">Vận chuyển</td><td style="padding:4px 0;">{order.ShippingMethod}</td></tr>
            """;

        var body = $"""
            <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="margin-bottom:18px;">
              <tr>
                <td style="font-family:Georgia,'Times New Roman',serif;font-size:20px;color:{RedDeep};">🔔&ensp;Đơn hàng mới</td>
                <td style="text-align:right;font-size:13px;color:{GreyText};">Mã đơn<br/><strong style="color:{Red};font-size:14px;">{Enc(order.OrderNumber)}</strong></td>
              </tr>
            </table>
            <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background:{Cream};border-radius:6px;">
              <tr><td style="padding:16px 18px;">
                <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="font-size:14px;">{infoRows}</table>
              </td></tr>
            </table>
            {ItemsTable(order.Items)}
            <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="font-size:14px;">
              <tr><td style="padding:14px 0;font-weight:bold;font-size:18px;border-top:1px solid {Border};">Tổng cộng</td><td style="padding:14px 0;font-weight:bold;font-size:18px;text-align:right;color:{Red};border-top:1px solid {Border};">{order.Total:N0}đ</td></tr>
            </table>
            <table role="presentation" cellpadding="0" cellspacing="0" style="margin:22px 0 0;">
              <tr><td style="border-radius:5px;background:{Red};">
                <a href="{adminUrl}" style="display:inline-block;padding:13px 26px;color:#ffffff;text-decoration:none;font-size:14px;font-weight:bold;">Xem đơn trong trang quản trị →</a>
              </td></tr>
            </table>
            """;
        return ($"[Đơn mới] {order.OrderNumber} — {order.FirstName} {order.LastName}",
            Shell($"Đơn hàng mới {order.OrderNumber} từ {order.FirstName} {order.LastName}", body));
    }
}
