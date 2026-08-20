using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Hoaii.Domain.Entities;

namespace Hoaii.Web.Services;

/// <summary>
/// Builds/verifies VNPAY "pay" requests per their published spec
/// (https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/pay.html). vnp_TxnRef is the order's own
/// Id (not OrderNumber) — plain numeric, trivially unique, and a callback can look the order up
/// with int.Parse instead of guessing how OrderNumber was mangled to fit "alphanumeric, no
/// duplicates per day".
/// </summary>
public class VnpayService(SiteSettingsService settings)
{
    private const string SandboxPayUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
    private const string ProductionPayUrl = "https://vnpayment.vn/paymentv2/vpcpay.html";

    public bool IsConfigured =>
        settings.Get(SiteSettingKeys.VnpayTmnCode).Length > 0 && settings.Get(SiteSettingKeys.VnpayHashSecret).Length > 0;

    /// <summary>Builds the full redirect URL for a freshly-placed, still-unpaid order.</summary>
    public string BuildPaymentUrl(Order order, string clientIp, string returnUrl)
    {
        var tmnCode = settings.Get(SiteSettingKeys.VnpayTmnCode);
        var hashSecret = settings.Get(SiteSettingKeys.VnpayHashSecret);
        var payUrl = settings.GetBool(SiteSettingKeys.VnpaySandbox) ? SandboxPayUrl : ProductionPayUrl;

        // GMT+7 regardless of what timezone the server itself runs in (everywhere else in this
        // app stores UtcNow, but VNPAY's own clock — and its ExpireDate check — is Vietnam time).
        var now = DateTime.UtcNow.AddHours(7);

        var fields = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["vnp_Version"] = "2.1.0",
            ["vnp_Command"] = "pay",
            ["vnp_TmnCode"] = tmnCode,
            ["vnp_Amount"] = ((long)(order.Total * 100)).ToString(CultureInfo.InvariantCulture),
            ["vnp_CurrCode"] = "VND",
            ["vnp_TxnRef"] = order.Id.ToString(CultureInfo.InvariantCulture),
            ["vnp_OrderInfo"] = StripDiacritics($"Thanh toan don hang {order.OrderNumber}"),
            ["vnp_OrderType"] = "other",
            ["vnp_Locale"] = "vn",
            ["vnp_ReturnUrl"] = returnUrl,
            ["vnp_IpAddr"] = clientIp,
            ["vnp_CreateDate"] = now.ToString("yyyyMMddHHmmss"),
            ["vnp_ExpireDate"] = now.AddMinutes(15).ToString("yyyyMMddHHmmss"),
        };

        var hashData = BuildHashData(fields);
        var secureHash = Hmac(hashData, hashSecret);
        return $"{payUrl}?{hashData}&vnp_SecureHash={secureHash}";
    }

    /// <summary>Recomputes the signature over everything VNPAY sent back (Return URL or IPN) and
    /// compares it to vnp_SecureHash. Both callback shapes carry the same fields, so one method
    /// covers both.</summary>
    public bool VerifySignature(IReadOnlyDictionary<string, string> query)
    {
        if (!query.TryGetValue("vnp_SecureHash", out var receivedHash) || receivedHash.Length == 0)
        {
            return false;
        }

        var fields = new SortedDictionary<string, string>(StringComparer.Ordinal);
        foreach (var (key, value) in query)
        {
            if (key is "vnp_SecureHash" or "vnp_SecureHashType")
            {
                continue;
            }
            fields[key] = value;
        }

        var hashData = BuildHashData(fields);
        var expected = Hmac(hashData, settings.Get(SiteSettingKeys.VnpayHashSecret));
        return string.Equals(expected, receivedHash, StringComparison.OrdinalIgnoreCase);
    }

    // Matches VNPAY's own PHP reference sample (urlencode on both key and value, joined by '&',
    // sorted by key) — including the fact that urlencode() encodes a space as '+', not "%20" like
    // Uri.EscapeDataString would. Getting this wrong produces a hash VNPAY silently rejects.
    private static string BuildHashData(SortedDictionary<string, string> fields) =>
        string.Join('&', fields.Select(kv => $"{System.Net.WebUtility.UrlEncode(kv.Key)}={System.Net.WebUtility.UrlEncode(kv.Value)}"));

    private static string Hmac(string data, string secret)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        var hash = HMACSHA512.HashData(keyBytes, dataBytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    /// <summary>vnp_OrderInfo must be Vietnamese-without-diacritics per the spec — reusing Slug's
    /// approach (strip combining marks after Unicode decomposition) but keeping spaces/case
    /// instead of dashing/lowercasing everything.</summary>
    private static string StripDiacritics(string value)
    {
        var decomposed = value.Replace('đ', 'd').Replace('Đ', 'D').Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(decomposed.Length);
        foreach (var c in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
}
