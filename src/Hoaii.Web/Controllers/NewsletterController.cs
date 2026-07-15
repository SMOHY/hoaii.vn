using System.ComponentModel.DataAnnotations;
using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Controllers;

/// <summary>
/// The footer sign-up form posts here from every page. Subscribers are stored in the DB
/// (deduplicated by email); actual email delivery is wired once SMTP is configured (Đợt 3.6).
/// </summary>
[Route("newsletter")]
public class NewsletterController(HoaiiDbContext db, ILogger<NewsletterController> logger) : Controller
{
    [HttpPost("subscribe")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Subscribe([Required, EmailAddress] string email, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return Request.Headers.XRequestedWith == "XMLHttpRequest"
                ? BadRequest(new { message = "Email không hợp lệ." })
                : SafeRedirect(returnUrl);
        }

        var normalized = email.Trim().ToLowerInvariant();
        if (!await db.NewsletterSubscribers.AnyAsync(s => s.Email == normalized))
        {
            db.NewsletterSubscribers.Add(new NewsletterSubscriber { Email = normalized, CreatedAt = DateTime.UtcNow });
            await db.SaveChangesAsync();
        }
        logger.LogInformation("Newsletter sign-up: {Email}", normalized);

        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
        {
            return Ok(new { message = "Cảm ơn bạn! Chúng tôi sẽ gửi tin mới nhất tới hộp thư của bạn." });
        }

        TempData["NewsletterSubscribed"] = true;
        return SafeRedirect(returnUrl);
    }

    private IActionResult SafeRedirect(string? returnUrl) =>
        !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? Redirect(returnUrl)
            : RedirectToAction("Index", "Home");
}
