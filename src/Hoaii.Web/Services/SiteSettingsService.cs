using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Hoaii.Web.Services;

/// <summary>
/// Well-known setting keys + their storefront defaults. The defaults are also what the
/// migration seeds, so a fresh database renders correctly before anyone opens the admin.
/// </summary>
public static class SiteSettingKeys
{
    public const string Hotline = "hotline";
    public const string AnnouncementText = "announcement_text";
    public const string ZaloPhone = "zalo_phone";
    public const string ZaloUrl = "zalo_url";
    public const string ContactEmail = "contact_email";
    public const string ContactPhone = "contact_phone";
    public const string Address = "address";
    public const string CompanyName = "company_name";
    public const string TaxCode = "tax_code";
    public const string OpeningHours = "opening_hours";
    public const string FacebookUrl = "facebook_url";
    public const string InstagramUrl = "instagram_url";
    public const string TiktokUrl = "tiktok_url";

    // Shipping (group "shipping").
    public const string ShippingInnerCity = "shipping_inner_city";
    public const string ShippingIntercity = "shipping_intercity";
    public const string FreeShipThreshold = "free_ship_threshold";

    // Payment (group "payment").
    public const string PayCodEnabled = "pay_cod_enabled";
    public const string PayBankEnabled = "pay_bank_enabled";
    public const string PayVnpayEnabled = "pay_vnpay_enabled";
    public const string BankName = "pay_bank_name";
    public const string BankAccountNumber = "pay_bank_account";
    public const string BankAccountHolder = "pay_bank_holder";
    public const string BankTransferNote = "pay_bank_note";

    // Email / SMTP (group "email").
    public const string SmtpHost = "smtp_host";
    public const string SmtpPort = "smtp_port";
    public const string SmtpUser = "smtp_user";
    public const string SmtpPassword = "smtp_password";
    public const string SmtpFromEmail = "smtp_from_email";
    public const string SmtpFromName = "smtp_from_name";
    public const string SmtpUseSsl = "smtp_use_ssl";

    /// <summary>Ordered for the admin forms; label + which admin screen (group) each field belongs to.</summary>
    public static readonly IReadOnlyList<(string Key, string Label, string Default, bool Multiline, string Group)> All =
    [
        (Hotline, "Hotline (thanh trên cùng)", "0941.686.682", false, "contact"),
        (AnnouncementText, "Dòng thông báo (thanh phụ)", "Hơn 100+ mẫu bánh và quà tặng độc đáo", false, "contact"),
        (CompanyName, "Tên công ty", "Công ty TNHH MTV Hoài", false, "contact"),
        (Address, "Địa chỉ", "945 Ngô Gia Tự, P. Việt Hưng, TP. Hà Nội", false, "contact"),
        (TaxCode, "Mã số thuế", "0101287214", false, "contact"),
        (ContactPhone, "Số điện thoại liên hệ", "0335006783", false, "contact"),
        (ContactEmail, "Email liên hệ", "hoai@gmail.com", false, "contact"),
        (ZaloPhone, "Số Zalo", "0335006783", false, "contact"),
        (ZaloUrl, "Link Zalo", "https://zalo.me/0335006783", false, "contact"),
        (OpeningHours, "Giờ làm việc", "09:00-18:00 (T2 - T7)", false, "contact"),
        (FacebookUrl, "Link Facebook", "https://facebook.com", false, "contact"),
        (InstagramUrl, "Link Instagram", "https://instagram.com", false, "contact"),
        (TiktokUrl, "Link TikTok", "https://tiktok.com", false, "contact"),

        (ShippingInnerCity, "Phí giao nội thành (đ)", "0", false, "shipping"),
        (ShippingIntercity, "Phí giao liên tỉnh (đ)", "0", false, "shipping"),
        (FreeShipThreshold, "Miễn phí ship từ (đ, 0 = tắt)", "0", false, "shipping"),

        (PayCodEnabled, "Cho phép thanh toán khi nhận hàng (COD)", "true", false, "payment"),
        (PayBankEnabled, "Cho phép chuyển khoản ngân hàng", "true", false, "payment"),
        (PayVnpayEnabled, "Bật cổng VNPay", "false", false, "payment"),
        (BankName, "Tên ngân hàng", "", false, "payment"),
        (BankAccountNumber, "Số tài khoản", "", false, "payment"),
        (BankAccountHolder, "Chủ tài khoản", "", false, "payment"),
        (BankTransferNote, "Ghi chú chuyển khoản (cú pháp)", "", true, "payment"),

        (SmtpHost, "Máy chủ SMTP", "", false, "email"),
        (SmtpPort, "Cổng SMTP", "587", false, "email"),
        (SmtpUser, "Tài khoản (email đăng nhập)", "", false, "email"),
        (SmtpPassword, "Mật khẩu / mật khẩu ứng dụng", "", false, "email"),
        (SmtpFromEmail, "Email gửi đi (From)", "", false, "email"),
        (SmtpFromName, "Tên hiển thị người gửi", "HOÀI", false, "email"),
        (SmtpUseSsl, "Dùng SSL/TLS", "true", false, "email"),
    ];

    public static IReadOnlyList<(string Key, string Label, string Default, bool Multiline, string Group)> InGroup(string group) =>
        All.Where(k => k.Group == group).ToList();
}

/// <summary>
/// Reads and writes site settings, caching the whole set so the storefront partials that call
/// it on every page don't hit the DB each time. The cache is dropped whenever a value changes.
/// </summary>
public class SiteSettingsService(HoaiiDbContext db, IMemoryCache cache)
{
    private const string CacheKey = "site_settings_all";

    private Dictionary<string, string> Load() =>
        cache.GetOrCreate(CacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            return db.SiteSettings.AsNoTracking().ToDictionary(s => s.Key, s => s.Value);
        })!;

    /// <summary>Value for a key, falling back to the coded default (never null).</summary>
    public string Get(string key)
    {
        var all = Load();
        if (all.TryGetValue(key, out var v) && !string.IsNullOrWhiteSpace(v))
        {
            return v;
        }
        return SiteSettingKeys.All.FirstOrDefault(k => k.Key == key).Default ?? "";
    }

    /// <summary>Value parsed as a VND amount; falls back to 0.</summary>
    public decimal GetDecimal(string key) =>
        decimal.TryParse(Get(key), System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : 0m;

    /// <summary>Value parsed as a boolean flag; "true"/"1" are true.</summary>
    public bool GetBool(string key)
    {
        var v = Get(key).Trim().ToLowerInvariant();
        return v is "true" or "1" or "on" or "yes";
    }

    public IReadOnlyDictionary<string, string> GetAllForEditing() =>
        GetForEditing(SiteSettingKeys.All.Select(k => k.Key));

    /// <summary>Current values (or defaults) for a specific set of keys — used by the group forms.</summary>
    public IReadOnlyDictionary<string, string> GetForEditing(IEnumerable<string> keys)
    {
        var stored = Load();
        return keys.ToDictionary(
            key => key,
            key => stored.TryGetValue(key, out var v)
                ? v
                : SiteSettingKeys.All.FirstOrDefault(k => k.Key == key).Default ?? "");
    }

    public async Task SaveAsync(IDictionary<string, string?> values)
    {
        var existing = await db.SiteSettings.ToDictionaryAsync(s => s.Key);
        foreach (var (key, value) in values)
        {
            if (!SiteSettingKeys.All.Any(k => k.Key == key))
            {
                continue; // ignore unknown keys
            }
            var v = value?.Trim() ?? "";
            if (existing.TryGetValue(key, out var row))
            {
                row.Value = v;
            }
            else
            {
                db.SiteSettings.Add(new SiteSetting { Key = key, Value = v });
            }
        }
        await db.SaveChangesAsync();
        cache.Remove(CacheKey);
    }
}
