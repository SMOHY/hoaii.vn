using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Hoaii.Tests;

/// <summary>
/// Signature bugs here mean either real money gets accepted without a valid VNPAY confirmation,
/// or valid payments get silently rejected — so this checks the hash round-trips exactly, not
/// just that the code runs.
/// </summary>
public class VnpayServiceTests
{
    private static (VnpayService Vnpay, HoaiiDbContext Db) NewVnpay(string tmnCode = "TESTCODE", string hashSecret = "TESTSECRET123")
    {
        var options = new DbContextOptionsBuilder<HoaiiDbContext>()
            .UseInMemoryDatabase($"vnpay-{Guid.NewGuid()}")
            .Options;
        var db = new HoaiiDbContext(options);
        db.SiteSettings.Add(new SiteSetting { Key = SiteSettingKeys.VnpayTmnCode, Value = tmnCode });
        db.SiteSettings.Add(new SiteSetting { Key = SiteSettingKeys.VnpayHashSecret, Value = hashSecret });
        db.SiteSettings.Add(new SiteSetting { Key = SiteSettingKeys.VnpaySandbox, Value = "true" });
        db.SaveChanges();
        var settings = new SiteSettingsService(db, new MemoryCache(new MemoryCacheOptions()));
        return (new VnpayService(settings), db);
    }

    private static Order SampleOrder() => new()
    {
        Id = 42,
        OrderNumber = "HD260818-0001",
        Email = "a@b.com",
        FirstName = "A",
        LastName = "B",
        Address = "123",
        ProvinceDistrictWard = "HN",
        Phone = "0900000000",
        Total = 1_806_000m,
    };

    /// <summary>VNPAY's own docs page shows vnp_OrderInfo=Thanh+toan+don+hang+%3A5 for the literal
    /// text "Thanh toan don hang :5" — pinning WebUtility.UrlEncode against their actual reference
    /// output (space to '+', not "%20") since a mismatch here silently breaks every real hash.</summary>
    [Fact]
    public void UrlEncode_matches_vnpays_own_published_example()
    {
        Assert.Equal("Thanh+toan+don+hang+%3A5", System.Net.WebUtility.UrlEncode("Thanh toan don hang :5"));
    }

    [Fact]
    public void BuildPaymentUrl_points_at_sandbox_and_carries_a_hash()
    {
        var (vnpay, _) = NewVnpay();
        var url = vnpay.BuildPaymentUrl(SampleOrder(), "13.160.92.202", "https://hoaii.vn/thanh-toan/vnpay/tra-ve");

        Assert.StartsWith("https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?", url);
        Assert.Contains("vnp_TmnCode=TESTCODE", url);
        Assert.Contains("vnp_Amount=180600000", url); // 1,806,000 * 100
        Assert.Contains("vnp_TxnRef=42", url); // Order.Id, not OrderNumber
        Assert.Contains("vnp_SecureHash=", url);
    }

    /// <summary>The exact scenario VerifySignature exists to catch: a callback whose amount or
    /// response code was tampered with after the hash was computed must never verify.</summary>
    [Fact]
    public void VerifySignature_rejects_a_tampered_amount()
    {
        var (vnpay, _) = NewVnpay();
        var url = vnpay.BuildPaymentUrl(SampleOrder(), "127.0.0.1", "https://hoaii.vn/thanh-toan/vnpay/tra-ve");
        var query = ParseQuery(url);

        // Simulate what a genuine VNPAY return/IPN adds on top of what we sent.
        query["vnp_ResponseCode"] = "00";
        query["vnp_TransactionNo"] = "14000123";
        query["vnp_TransactionStatus"] = "00";
        query["vnp_BankCode"] = "NCB";
        query["vnp_PayDate"] = "20260818120000";

        // This is the ONE query VNPAY would actually send (hash covers all fields including the
        // response ones) — recompute what a real callback's hash would be by re-signing.
        var resigned = Resign(query, "TESTSECRET123");
        Assert.True(vnpay.VerifySignature(resigned));

        // Now tamper with the amount after the fact, as an attacker forging a callback would.
        var tampered = new Dictionary<string, string>(resigned) { ["vnp_Amount"] = "100" };
        Assert.False(vnpay.VerifySignature(tampered));
    }

    [Fact]
    public void VerifySignature_rejects_wrong_secret()
    {
        var (vnpay, _) = NewVnpay(hashSecret: "TESTSECRET123");
        var url = vnpay.BuildPaymentUrl(SampleOrder(), "127.0.0.1", "https://hoaii.vn/thanh-toan/vnpay/tra-ve");
        var query = ParseQuery(url);
        query["vnp_ResponseCode"] = "00";

        var signedWithWrongSecret = Resign(query, "SOME-OTHER-SECRET");
        Assert.False(vnpay.VerifySignature(signedWithWrongSecret));
    }

    private static Dictionary<string, string> ParseQuery(string url)
    {
        var qs = url[(url.IndexOf('?') + 1)..];
        return qs.Split('&').Select(p => p.Split('=', 2))
            .ToDictionary(p => p[0], p => Uri.UnescapeDataString(p[1].Replace('+', ' ')));
    }

    /// <summary>Re-signs a param set exactly the way VnpayService itself would — used only to
    /// produce a realistic "genuine callback" fixture for the tests above, not exercised by
    /// production code.</summary>
    private static Dictionary<string, string> Resign(Dictionary<string, string> query, string secret)
    {
        var withoutHash = query.Where(kv => kv.Key != "vnp_SecureHash").ToDictionary(kv => kv.Key, kv => kv.Value);
        var sorted = new SortedDictionary<string, string>(withoutHash, StringComparer.Ordinal);
        var hashData = string.Join('&', sorted.Select(kv => $"{System.Net.WebUtility.UrlEncode(kv.Key)}={System.Net.WebUtility.UrlEncode(kv.Value)}"));
        var hash = System.Security.Cryptography.HMACSHA512.HashData(System.Text.Encoding.UTF8.GetBytes(secret), System.Text.Encoding.UTF8.GetBytes(hashData));
        withoutHash["vnp_SecureHash"] = Convert.ToHexString(hash).ToLowerInvariant();
        return withoutHash;
    }
}
