using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Models.Page;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Controllers;

public class PageController(HoaiiDbContext db) : Controller
{

    public async Task<IActionResult> AboutUs()
    {
        // The customer-logo strip reuses the same list the homepage manages (no more duplicate).
        ViewBag.CustomerLogos = await db.HomeCustomerLogos
            .OrderBy(l => l.SortOrder).ThenBy(l => l.Id)
            .Select(l => l.LogoKey)
            .ToListAsync();
        return View();
    }

    public async Task<IActionResult> Partners()
    {
        ViewBag.PartnerLogos = await db.PartnerLogos
            .OrderBy(l => l.SortOrder).ThenBy(l => l.Id)
            .Select(l => l.LogoKey)
            .ToListAsync();
        return View(new WholesaleFormModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Partners(WholesaleFormModel form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        db.WholesaleLeads.Add(new WholesaleLead
        {
            FirstName = form.FirstName,
            LastName = form.LastName,
            Email = form.Email,
            Phone = form.Phone,
            PostalCode = form.PostalCode,
            CompanyName = form.CompanyName,
            RequestType = form.RequestType,
            Message = form.Message,
            CreatedAt = DateTime.UtcNow,
        });
        await db.SaveChangesAsync();

        TempData["WholesaleSubmitted"] = true;
        return RedirectToAction(nameof(Partners));
    }

    public IActionResult Contact()
    {
        return View(new ContactFormModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(ContactFormModel form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        db.ContactSubmissions.Add(new ContactSubmission
        {
            FirstName = form.FirstName,
            LastName = form.LastName,
            Email = form.Email,
            Phone = form.Phone,
            Message = form.Message,
            CreatedAt = DateTime.UtcNow,
        });
        await db.SaveChangesAsync();

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
