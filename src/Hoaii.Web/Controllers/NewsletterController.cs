using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Hoaii.Web.Controllers;

/// <summary>
/// The footer sign-up form posts here from every page. Like the contact and wholesale forms,
/// there is no email/CRM integration configured yet, so this only acknowledges receipt —
/// wire up real delivery before production.
/// </summary>
[Route("newsletter")]
public class NewsletterController(ILogger<NewsletterController> logger) : Controller
{
    [HttpPost("subscribe")]
    [ValidateAntiForgeryToken]
    public IActionResult Subscribe([Required, EmailAddress] string email, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return Request.Headers.XRequestedWith == "XMLHttpRequest"
                ? BadRequest(new { message = "Email không hợp lệ." })
                : SafeRedirect(returnUrl);
        }

        logger.LogInformation("Newsletter sign-up: {Email}", email);

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
