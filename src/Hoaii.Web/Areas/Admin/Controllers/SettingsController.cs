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
        var fields = SiteSettingKeys.InGroup("contact");
        ViewBag.Fields = fields;
        ViewBag.FormAction = "/admin/cai-dat";
        ViewBag.Intro = "Thông tin liên hệ và mạng xã hội hiển thị khắp trang: thanh trên cùng, chân trang, popup Zalo, trang liên hệ.";
        return View("Fields", settings.GetForEditing(fields.Select(f => f.Key)));
    }

    [HttpPost("/admin/cai-dat")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(Dictionary<string, string?> settings_)
    {
        await settings.SaveAsync(settings_.ToDictionary(kv => kv.Key, kv => kv.Value));
        auth.Audit("Cập nhật cài đặt", nameof(SiteSetting));
        await Db.SaveChangesAsync();
        Ok("Đã lưu cài đặt.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/admin/van-chuyen")]
    public IActionResult Shipping()
    {
        var fields = SiteSettingKeys.InGroup("shipping");
        ViewBag.Fields = fields;
        ViewBag.FormAction = "/admin/van-chuyen";
        ViewBag.Intro = "Phí vận chuyển áp ở bước thanh toán theo khu vực khách chọn. Đặt ngưỡng miễn phí ship để tự động miễn phí khi đơn đủ lớn.";
        return View("Fields", settings.GetForEditing(fields.Select(f => f.Key)));
    }

    [HttpPost("/admin/van-chuyen")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveShipping(Dictionary<string, string?> settings_)
    {
        await settings.SaveAsync(settings_.ToDictionary(kv => kv.Key, kv => kv.Value));
        auth.Audit("Cập nhật vận chuyển", nameof(SiteSetting));
        await Db.SaveChangesAsync();
        Ok("Đã lưu cấu hình vận chuyển.");
        return RedirectToAction(nameof(Shipping));
    }
}
