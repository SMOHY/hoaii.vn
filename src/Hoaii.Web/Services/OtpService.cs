using Microsoft.Extensions.Caching.Memory;

namespace Hoaii.Web.Services;

/// <summary>
/// Generates and validates one-time login codes for the passwordless email+OTP flow.
/// The code is emailed via <see cref="EmailSender"/>; until SMTP is configured, the sender runs
/// in log mode so the code still appears in the application log during development.
/// </summary>
public class OtpService(IMemoryCache cache, EmailSender emailSender, ILogger<OtpService> logger)
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(5);

    private static string CacheKey(string email) => $"otp:{email.Trim().ToLowerInvariant()}";

    public async Task<string> SendAsync(string email)
    {
        var code = Random.Shared.Next(0, 1_000_000).ToString("D6");
        cache.Set(CacheKey(email), code, Ttl);

        var body = $"""
            <p>Xin chào,</p>
            <p>Mã đăng nhập HOÀI của bạn là:</p>
            <p style="font-size:28px;font-weight:700;letter-spacing:4px;">{code}</p>
            <p>Mã có hiệu lực trong 5 phút. Nếu bạn không yêu cầu, hãy bỏ qua email này.</p>
            """;
        var result = await emailSender.SendAsync(email, "Mã đăng nhập HOÀI", body);
        if (!result.Delivered)
        {
            logger.LogWarning("[OTP] Mã cho {Email} là {Code} (chưa gửi email — SMTP chưa cấu hình).", email, code);
        }

        return code;
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
