using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Models.Page;
using Hoaii.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Controllers;

public class PageController(HoaiiDbContext db, SiteSettingsService settings) : Controller
{

    /// <summary>Legal owner-info disclosure page. Reads live from Cài đặt trang instead of
    /// duplicating the values into a PolicyPage, so it never drifts from what's set in admin.</summary>
    public IActionResult OwnerInfo()
    {
        return View(new OwnerInfoViewModel
        {
            CompanyName = settings.Get(SiteSettingKeys.CompanyName),
            Address = settings.Get(SiteSettingKeys.Address),
            TaxCode = settings.Get(SiteSettingKeys.TaxCode),
            Phone = settings.Get(SiteSettingKeys.ContactPhone),
            Email = settings.Get(SiteSettingKeys.ContactEmail),
            RepresentativeName = settings.Get(SiteSettingKeys.RepresentativeName),
            RepresentativeTitle = settings.Get(SiteSettingKeys.RepresentativeTitle),
            RepresentativeName2 = settings.Get(SiteSettingKeys.RepresentativeName2),
            RepresentativeTitle2 = settings.Get(SiteSettingKeys.RepresentativeTitle2),
            RegistrationNumber = settings.Get(SiteSettingKeys.RegistrationNumber),
            RegistrationFirstDate = settings.Get(SiteSettingKeys.RegistrationFirstDate),
            RegistrationAmendDate = settings.Get(SiteSettingKeys.RegistrationAmendDate),
            RegistrationIssuedBy = settings.Get(SiteSettingKeys.RegistrationIssuedBy),
            WebsiteOperatorName = settings.Get(SiteSettingKeys.WebsiteOperatorName),
        });
    }

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
