using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Controllers;

/// <summary>
/// Handles both VNPAY callback channels. Both funnel through the same ConfirmAsync — verify
/// signature, look up the order by vnp_TxnRef (= Order.Id), check the amount, then flip
/// PaymentStatus exactly once. Never trust vnp_ResponseCode without a valid signature first.
///
/// IPN is server-to-server (VNPAY's own infra calling us) and is the authoritative source per
/// VNPAY's spec — it can reach a real HTTPS domain but never a local dev machine. Return is the
/// customer's own browser bouncing back, which works everywhere (including localhost) but per
/// VNPAY's docs must be treated as UX-only. Confirming from both is deliberate: locally, only
/// Return ever fires; in production both fire and the second one just sees "already confirmed".
/// </summary>
public class VnpayController(HoaiiDbContext db, VnpayService vnpay, ILogger<VnpayController> logger) : Controller
{
    public async Task<IActionResult> Return()
    {
        var query = Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
        logger.LogInformation("VNPAY Return: {Query}", Request.QueryString);
        var result = await ConfirmAsync(query);
        logger.LogInformation("VNPAY Return result: RspCode={RspCode} OrderId={OrderId} Success={Success}",
            result.RspCode, result.Order?.Id, result.Success);
        if (result.Order is null)
        {
            return RedirectToAction("Index", "Checkout");
        }
        return RedirectToAction("Confirmation", "Checkout",
            new { orderNumber = result.Order.OrderNumber, vnpay = result.Success ? "ok" : "failed" });
    }

    [HttpGet]
    public async Task<IActionResult> Ipn()
    {
        // VNPAY's own spec (RspCode 99 = "Unknow error") requires the IPN endpoint to always
        // answer with well-formed JSON, even when something inside ConfirmAsync blows up (a DB
        // hiccup, a malformed query VNPAY never actually sends, etc) — an unhandled exception
        // here would otherwise surface as a bare 500 with no body, which VNPAY can't parse.
        VnpayConfirmResult result;
        try
        {
            var query = Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
            logger.LogInformation("VNPAY IPN: {Query}", Request.QueryString);
            result = await ConfirmAsync(query);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "VNPAY IPN: unhandled error while processing {Query}", Request.QueryString);
            result = new VnpayConfirmResult(null, false, "99", "Unknow error");
        }
        logger.LogInformation("VNPAY IPN result: RspCode={RspCode} OrderId={OrderId} Success={Success}",
            result.RspCode, result.Order?.Id, result.Success);

        // Content(), not Json() — the default JSON serializer camel-cases property names
        // ("rspCode"), but VNPAY's spec requires the exact casing "RspCode"/"Message" and there's
        // no guarantee their parser is case-insensitive.
        var json = $$"""{"RspCode":"{{result.RspCode}}","Message":"{{result.Message}}"}""";
        return Content(json, "application/json");
    }

    private async Task<VnpayConfirmResult> ConfirmAsync(Dictionary<string, string> query)
    {
        if (!vnpay.VerifySignature(query))
        {
            return new VnpayConfirmResult(null, false, "97", "Invalid signature");
        }

        if (!query.TryGetValue("vnp_TxnRef", out var txnRefStr) || !int.TryParse(txnRefStr, out var orderId))
        {
            return new VnpayConfirmResult(null, false, "01", "Order not found");
        }

        var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order is null)
        {
            return new VnpayConfirmResult(null, false, "01", "Order not found");
        }

        if (!query.TryGetValue("vnp_Amount", out var amountStr) || !long.TryParse(amountStr, out var amount)
            || amount != (long)(order.Total * 100))
        {
            return new VnpayConfirmResult(order, false, "04", "Invalid amount");
        }

        query.TryGetValue("vnp_ResponseCode", out var responseCode);
        var success = responseCode == "00";

        // Already settled by the other channel (Return vs IPN, whichever landed first) — not an
        // error, just tell VNPAY to stop retrying and report back whatever the outcome already was.
        if (order.PaymentStatus != PaymentStatus.Unpaid)
        {
            return new VnpayConfirmResult(order, order.PaymentStatus == PaymentStatus.Paid, "02", "Order already confirmed");
        }

        query.TryGetValue("vnp_TransactionNo", out var txnNo);
        query.TryGetValue("vnp_BankCode", out var bankCode);
        order.AdminNote = AppendNote(order.AdminNote, success
            ? $"VNPAY: đã thanh toán — mã GD {txnNo}, ngân hàng {bankCode}."
            : $"VNPAY: thanh toán không thành công (mã lỗi {responseCode}).");
        if (success)
        {
            order.PaymentStatus = PaymentStatus.Paid;
        }
        order.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return new VnpayConfirmResult(order, success, "00", "Confirm Success");
    }

    private static string? AppendNote(string? existing, string line) =>
        string.IsNullOrWhiteSpace(existing) ? line : $"{existing}\n{line}";

    private record VnpayConfirmResult(Order? Order, bool Success, string RspCode, string Message);
}
