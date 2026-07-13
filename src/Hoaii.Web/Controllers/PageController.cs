using Hoaii.Web.Models.Page;
using Microsoft.AspNetCore.Mvc;

namespace Hoaii.Web.Controllers;

public class PageController : Controller
{
    private static readonly Dictionary<string, string> PolicyTitles = new()
    {
        ["trao-doi"] = "Chính sách trao đổi",
        ["giao-hang"] = "Chính sách giao hàng",
        ["dieu-khoan-su-dung"] = "Điều khoản sử dụng",
        ["bao-mat"] = "Chính sách bảo vệ dữ liệu cá nhân",
    };

    public IActionResult AboutUs()
    {
        return View();
    }

    public IActionResult Partners()
    {
        return View(new WholesaleFormModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Partners(WholesaleFormModel form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        // No email/CRM integration configured yet — see design-specs notes; this simply
        // acknowledges receipt. Wire up real delivery (email/CRM) before production.
        TempData["WholesaleSubmitted"] = true;
        return RedirectToAction(nameof(Partners));
    }

    public IActionResult Contact()
    {
        return View(new ContactFormModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Contact(ContactFormModel form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        TempData["ContactSubmitted"] = true;
        return RedirectToAction(nameof(Contact));
    }

    public IActionResult Policy(string slug)
    {
        if (!PolicyTitles.TryGetValue(slug, out var title))
        {
            return NotFound();
        }

        return View(new PolicyPageViewModel
        {
            Title = title,
            BreadcrumbLabel = $"Trang chủ/{title}",
        });
    }
}
