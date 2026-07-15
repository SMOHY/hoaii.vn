using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Services;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Hoaii.Web.Areas.Admin.Controllers;

public class SettingsController(HoaiiDbContext db, SiteSettingsService settings, AdminAuthService auth)
    : BaseAdminController(db)
{
    [HttpGet("/admin/cai-dat")]
    public IActionResult Index()
    {
        ViewBag.Fields = SiteSettingKeys.All;
        return View(settings.GetAllForEditing());
    }

    [HttpPost("/admin/cai-dat")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(Dictionary<string, string?> settings_)
    {
        // Model-bound as settings_[key] = value from inputs named settings_[hotline] etc.
        await settings.SaveAsync(settings_.ToDictionary(kv => kv.Key, kv => kv.Value));
        auth.Audit("Cập nhật cài đặt", nameof(SiteSetting));
        await Db.SaveChangesAsync();
        Ok("Đã lưu cài đặt.");
        return RedirectToAction(nameof(Index));
    }
}
