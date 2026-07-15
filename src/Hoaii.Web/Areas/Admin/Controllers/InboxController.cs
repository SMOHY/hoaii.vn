using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Areas.Admin.Controllers;

/// <summary>The three storefront forms (contact, wholesale, newsletter) now land here instead of
/// being discarded.</summary>
public class InboxController(HoaiiDbContext db, AdminAuthService auth) : BaseAdminController(db)
{
    [HttpGet("/admin/hop-thu")]
    public async Task<IActionResult> Contacts()
    {
        await SetCountsAsync();
        return View(await Db.ContactSubmissions.OrderByDescending(c => c.CreatedAt).ToListAsync());
    }

    [HttpGet("/admin/hop-thu/ban-buon")]
    public async Task<IActionResult> Wholesale()
    {
        await SetCountsAsync();
        return View(await Db.WholesaleLeads.OrderByDescending(w => w.CreatedAt).ToListAsync());
    }

    [HttpGet("/admin/hop-thu/newsletter")]
    public async Task<IActionResult> Newsletter()
    {
        await SetCountsAsync();
        return View(await Db.NewsletterSubscribers.OrderByDescending(n => n.CreatedAt).ToListAsync());
    }

    [HttpPost("/admin/hop-thu/lien-he/{id:int}/xu-ly")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ContactToggle(int id)
    {
        var c = await Db.ContactSubmissions.FindAsync(id);
        if (c is null) return NotFound();
        c.IsHandled = !c.IsHandled;
        await Db.SaveChangesAsync();
        Ok(c.IsHandled ? "Đã đánh dấu xử lý." : "Đã bỏ đánh dấu.");
        return RedirectToAction(nameof(Contacts));
    }

    [HttpPost("/admin/hop-thu/lien-he/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ContactDelete(int id)
    {
        var c = await Db.ContactSubmissions.FindAsync(id);
        if (c is null) return NotFound();
        Db.ContactSubmissions.Remove(c);
        auth.Audit("Xóa liên hệ", nameof(ContactSubmission), id, c.Email);
        await Db.SaveChangesAsync();
        Ok("Đã xóa.");
        return RedirectToAction(nameof(Contacts));
    }

    [HttpPost("/admin/hop-thu/ban-buon/{id:int}/xu-ly")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> WholesaleToggle(int id)
    {
        var w = await Db.WholesaleLeads.FindAsync(id);
        if (w is null) return NotFound();
        w.IsHandled = !w.IsHandled;
        await Db.SaveChangesAsync();
        Ok(w.IsHandled ? "Đã đánh dấu xử lý." : "Đã bỏ đánh dấu.");
        return RedirectToAction(nameof(Wholesale));
    }

    [HttpPost("/admin/hop-thu/ban-buon/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> WholesaleDelete(int id)
    {
        var w = await Db.WholesaleLeads.FindAsync(id);
        if (w is null) return NotFound();
        Db.WholesaleLeads.Remove(w);
        auth.Audit("Xóa yêu cầu bán buôn", nameof(WholesaleLead), id, w.Email);
        await Db.SaveChangesAsync();
        Ok("Đã xóa.");
        return RedirectToAction(nameof(Wholesale));
    }

    [HttpPost("/admin/hop-thu/newsletter/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NewsletterDelete(int id)
    {
        var n = await Db.NewsletterSubscribers.FindAsync(id);
        if (n is null) return NotFound();
        Db.NewsletterSubscribers.Remove(n);
        auth.Audit("Xóa người đăng ký", nameof(NewsletterSubscriber), id, n.Email);
        await Db.SaveChangesAsync();
        Ok("Đã xóa.");
        return RedirectToAction(nameof(Newsletter));
    }

    private async Task SetCountsAsync()
    {
        ViewBag.ContactCount = await Db.ContactSubmissions.CountAsync();
        ViewBag.ContactNew = await Db.ContactSubmissions.CountAsync(c => !c.IsHandled);
        ViewBag.WholesaleCount = await Db.WholesaleLeads.CountAsync();
        ViewBag.WholesaleNew = await Db.WholesaleLeads.CountAsync(w => !w.IsHandled);
        ViewBag.NewsletterCount = await Db.NewsletterSubscribers.CountAsync();
    }
}
