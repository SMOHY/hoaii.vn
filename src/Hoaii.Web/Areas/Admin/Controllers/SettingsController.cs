using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Services;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Hoaii.Web.Areas.Admin.Controllers;

public class SettingsController(HoaiiDbContext db, SiteSettingsService settings, AdminAuthService auth, EmailSender emailSender)
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

    [HttpGet("/admin/thanh-toan")]
    public IActionResult Payment()
    {
        var keys = SiteSettingKeys.InGroup("payment").Select(f => f.Key);
        return View(settings.GetForEditing(keys));
    }

    [HttpPost("/admin/thanh-toan")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SavePayment(Dictionary<string, string?> settings_)
    {
        // Unchecked checkboxes don't post — force the three flags to explicit true/false.
        foreach (var flag in new[] { SiteSettingKeys.PayCodEnabled, SiteSettingKeys.PayBankEnabled, SiteSettingKeys.PayVnpayEnabled })
        {
            settings_[flag] = settings_.TryGetValue(flag, out var v) && v == "true" ? "true" : "false";
        }
        await settings.SaveAsync(settings_.ToDictionary(kv => kv.Key, kv => kv.Value));
        auth.Audit("Cập nhật thanh toán", nameof(SiteSetting));
        await Db.SaveChangesAsync();
        Ok("Đã lưu cấu hình thanh toán.");
        return RedirectToAction(nameof(Payment));
    }

    [HttpGet("/admin/email")]
    public IActionResult Email()
    {
        var keys = SiteSettingKeys.InGroup("email").Select(f => f.Key);
        return View(settings.GetForEditing(keys));
    }

    [HttpPost("/admin/email")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveEmail(Dictionary<string, string?> settings_)
    {
        settings_[SiteSettingKeys.SmtpUseSsl] =
            settings_.TryGetValue(SiteSettingKeys.SmtpUseSsl, out var v) && v == "true" ? "true" : "false";
        await settings.SaveAsync(settings_.ToDictionary(kv => kv.Key, kv => kv.Value));
        auth.Audit("Cập nhật email", nameof(SiteSetting));
        await Db.SaveChangesAsync();
        Ok("Đã lưu cấu hình email.");
        return RedirectToAction(nameof(Email));
    }

    [HttpPost("/admin/email/gui-thu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendTest(string testEmail)
    {
        if (string.IsNullOrWhiteSpace(testEmail))
        {
            Fail("Nhập email để gửi thử.");
            return RedirectToAction(nameof(Email));
        }
        var result = await emailSender.SendAsync(testEmail.Trim(), "Email thử từ HOÀI",
            "<p>Đây là email thử. Nếu bạn nhận được, cấu hình SMTP đã hoạt động.</p>");
        if (result.Delivered) Ok($"Đã gửi email thử tới {testEmail}.");
        else if (result.Ok) Fail("SMTP chưa cấu hình — email chỉ được ghi vào log.");
        else Fail($"Gửi thất bại: {result.Error}");
        return RedirectToAction(nameof(Email));
    }
}
