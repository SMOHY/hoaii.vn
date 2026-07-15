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

    /// <summary>Ordered for the admin form; label shown next to each field.</summary>
    public static readonly IReadOnlyList<(string Key, string Label, string Default, bool Multiline)> All =
    [
        (Hotline, "Hotline (thanh trên cùng)", "0941.686.682", false),
        (AnnouncementText, "Dòng thông báo (thanh phụ)", "Hơn 100+ mẫu bánh và quà tặng độc đáo", false),
        (CompanyName, "Tên công ty", "Công ty TNHH MTV Hoài", false),
        (Address, "Địa chỉ", "945 Ngô Gia Tự, P. Việt Hưng, TP. Hà Nội", false),
        (TaxCode, "Mã số thuế", "0101287214", false),
        (ContactPhone, "Số điện thoại liên hệ", "0335006783", false),
        (ContactEmail, "Email liên hệ", "hoai@gmail.com", false),
        (ZaloPhone, "Số Zalo", "0335006783", false),
        (ZaloUrl, "Link Zalo", "https://zalo.me/0335006783", false),
        (OpeningHours, "Giờ làm việc", "09:00-18:00 (T2 - T7)", false),
        (FacebookUrl, "Link Facebook", "https://facebook.com", false),
        (InstagramUrl, "Link Instagram", "https://instagram.com", false),
        (TiktokUrl, "Link TikTok", "https://tiktok.com", false),
    ];
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

    public IReadOnlyDictionary<string, string> GetAllForEditing()
    {
        var stored = Load();
        return SiteSettingKeys.All.ToDictionary(
            k => k.Key,
            k => stored.TryGetValue(k.Key, out var v) ? v : k.Default);
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
