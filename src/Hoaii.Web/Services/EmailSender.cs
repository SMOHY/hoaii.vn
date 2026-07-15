using System.Net;
using System.Net.Mail;

namespace Hoaii.Web.Services;

/// <summary>
/// Sends transactional email via the SMTP account configured in admin. When no SMTP host is set
/// the sender runs in "log mode" — it writes the message to the application log instead of
/// throwing, so OTP and order-confirmation flows keep working on a fresh install. The moment the
/// shop fills in SMTP details, real delivery starts with no code change.
/// </summary>
public class EmailSender(SiteSettingsService settings, ILogger<EmailSender> logger)
{
    public bool IsConfigured => !string.IsNullOrWhiteSpace(settings.Get(SiteSettingKeys.SmtpHost));

    public record SendResult(bool Ok, bool Delivered, string? Error);

    public async Task<SendResult> SendAsync(string toEmail, string subject, string htmlBody)
    {
        if (string.IsNullOrWhiteSpace(toEmail))
        {
            return new SendResult(false, false, "Thiếu địa chỉ nhận.");
        }

        if (!IsConfigured)
        {
            // Log mode — no SMTP configured yet.
            logger.LogWarning("[EMAIL:log-mode] Tới {To} — {Subject}\n{Body}", toEmail, subject, htmlBody);
            return new SendResult(true, false, null);
        }

        try
        {
            var host = settings.Get(SiteSettingKeys.SmtpHost);
            var port = (int)settings.GetDecimal(SiteSettingKeys.SmtpPort);
            if (port <= 0) port = 587;
            var user = settings.Get(SiteSettingKeys.SmtpUser);
            var password = settings.Get(SiteSettingKeys.SmtpPassword);
            var fromEmail = settings.Get(SiteSettingKeys.SmtpFromEmail);
            if (string.IsNullOrWhiteSpace(fromEmail)) fromEmail = user;
            var fromName = settings.Get(SiteSettingKeys.SmtpFromName);

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = settings.GetBool(SiteSettingKeys.SmtpUseSsl),
                DeliveryMethod = SmtpDeliveryMethod.Network,
            };
            if (!string.IsNullOrWhiteSpace(user))
            {
                client.Credentials = new NetworkCredential(user, password);
            }

            using var message = new MailMessage
            {
                From = new MailAddress(fromEmail, string.IsNullOrWhiteSpace(fromName) ? fromEmail : fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true,
            };
            message.To.Add(toEmail);

            await client.SendMailAsync(message);
            return new SendResult(true, true, null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Gửi email tới {To} thất bại", toEmail);
            return new SendResult(false, false, ex.Message);
        }
    }
}
