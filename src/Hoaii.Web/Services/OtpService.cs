using Microsoft.Extensions.Caching.Memory;

namespace Hoaii.Web.Services;

/// <summary>
/// Generates and validates one-time login codes for the passwordless email+OTP flow.
/// DEV-ONLY DELIVERY: no email/SMS provider is wired up yet, so the generated code is
/// written to the application log instead of actually being emailed. Wire up a real
/// mail provider (SendGrid, SES, SMTP, etc.) in <see cref="SendAsync"/> before production.
/// </summary>
public class OtpService(IMemoryCache cache, ILogger<OtpService> logger)
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(5);

    private static string CacheKey(string email) => $"otp:{email.Trim().ToLowerInvariant()}";

    public Task<string> SendAsync(string email)
    {
        var code = Random.Shared.Next(0, 1_000_000).ToString("D6");
        cache.Set(CacheKey(email), code, Ttl);

        // Placeholder delivery channel — see class remarks.
        logger.LogWarning("[DEV OTP] Mã xác thực cho {Email} là {Code} (hết hạn sau 5 phút)", email, code);

        return Task.FromResult(code);
    }

    public bool Verify(string email, string code)
    {
        if (!cache.TryGetValue(CacheKey(email), out string? expected) || expected is null)
        {
            return false;
        }

        var isValid = expected == code.Trim();
        if (isValid)
        {
            cache.Remove(CacheKey(email));
        }

        return isValid;
    }
}
