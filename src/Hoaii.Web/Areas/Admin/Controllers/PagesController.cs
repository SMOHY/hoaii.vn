using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Services;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Areas.Admin.Controllers;

/// <summary>Edits the fixed-layout content pages (Về chúng tôi, Hợp tác) via the reusable
/// PageContent store, plus the partner-logo list.</summary>
public class PagesController(HoaiiDbContext db, PageContentService content, AdminAuthService auth) : BaseAdminController(db)
{
    [HttpGet("/admin/trang/gioi-thieu")]
    public IActionResult About()
    {
        ViewBag.Fields = PageContentKeys.ForPage(PageContentKeys.About);
        ViewBag.FormAction = "/admin/trang/gioi-thieu";
        ViewBag.Title = "Trang Về chúng tôi";
        ViewBag.Note = "Dải logo khách hàng dùng chung với Trang chủ — sửa ở mục Trang chủ.";
        return View("Fields", content.GetForEditing(PageContentKeys.About));
    }

    [HttpPost("/admin/trang/gioi-thieu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveAbout(Dictionary<string, string?> f)
    {
        await content.SaveAsync(PageContentKeys.About, f.ToDictionary(kv => kv.Key, kv => kv.Value));
        auth.Audit("Sửa trang Về chúng tôi", nameof(PageContent));
        await Db.SaveChangesAsync();
        Ok("Đã lưu trang Về chúng tôi.");
        return RedirectToAction(nameof(About));
    }

    [HttpGet("/admin/trang/hop-tac")]
    public async Task<IActionResult> Partners()
    {
        ViewBag.Fields = PageContentKeys.ForPage(PageContentKeys.Partners);
        ViewBag.FormAction = "/admin/trang/hop-tac";
        ViewBag.Title = "Trang Hợp tác";
        ViewBag.Logos = await Db.PartnerLogos.OrderBy(l => l.SortOrder).ThenBy(l => l.Id).ToListAsync();
        return View("Fields", content.GetForEditing(PageContentKeys.Partners));
    }

    [HttpPost("/admin/trang/hop-tac")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SavePartners(Dictionary<string, string?> f)
    {
        await content.SaveAsync(PageContentKeys.Partners, f.ToDictionary(kv => kv.Key, kv => kv.Value));
        auth.Audit("Sửa trang Hợp tác", nameof(PageContent));
        await Db.SaveChangesAsync();
        Ok("Đã lưu trang Hợp tác.");
        return RedirectToAction(nameof(Partners));
    }

    [HttpPost("/admin/trang/hop-tac/logo/them")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PartnerLogoAdd(string logoKey)
    {
        if (!string.IsNullOrWhiteSpace(logoKey))
        {
            var max = await Db.PartnerLogos.MaxAsync(l => (int?)l.SortOrder) ?? -1;
            Db.PartnerLogos.Add(new PartnerLogo { LogoKey = logoKey.Trim(), SortOrder = max + 1 });
            await Db.SaveChangesAsync();
            Ok("Đã thêm logo đối tác.");
        }
        return RedirectToAction(nameof(Partners));
    }

    [HttpPost("/admin/trang/hop-tac/logo/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PartnerLogoDelete(int id)
    {
        var logo = await Db.PartnerLogos.FindAsync(id);
        if (logo is null) return NotFound();
        Db.PartnerLogos.Remove(logo);
        await Db.SaveChangesAsync();
        Ok("Đã xóa logo đối tác.");
        return RedirectToAction(nameof(Partners));
    }
}
