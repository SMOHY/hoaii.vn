using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Models.Page;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Controllers;

public class PageController(HoaiiDbContext db) : Controller
{

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

    public async Task<IActionResult> Policy(string slug)
    {
        var page = await db.PolicyPages
            .AsNoTracking()
            .Include(p => p.Blocks.OrderBy(b => b.SortOrder))
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished);

        if (page is null)
        {
            return NotFound();
        }

        return View(new PolicyPageViewModel
        {
            Title = page.Title,
            NavLabel = page.NavLabel,
            BreadcrumbLabel = page.BreadcrumbLabel,
            Blocks = page.Blocks
                .Select(b => new Models.Page.PolicyBlock
                {
                    Kind = (Models.Page.PolicyBlockKind)(int)b.Kind,
                    Text = b.Text,
                })
                .ToList(),
        });
    }
}
