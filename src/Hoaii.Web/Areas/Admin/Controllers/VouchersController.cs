using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Areas.Admin.Controllers;

public class VouchersController(HoaiiDbContext db, AdminAuthService auth) : BaseAdminController(db)
{
    [HttpGet("/admin/voucher")]
    public async Task<IActionResult> Index()
    {
        var vouchers = await Db.Vouchers.OrderByDescending(v => v.Id).ToListAsync();
        return View(vouchers);
    }

    [HttpGet("/admin/voucher/them")]
    public IActionResult Create() => View("Edit", new Voucher { Code = "" });

    [HttpGet("/admin/voucher/{id:int}/sua")]
    public async Task<IActionResult> Edit(int id)
    {
        var v = await Db.Vouchers.FindAsync(id);
        return v is null ? NotFound() : View(v);
    }

    [HttpPost("/admin/voucher/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(
        int id, string code, string? label, string? tag, VoucherType type, decimal value,
        decimal minOrderAmount, decimal? maxDiscountAmount, int? usageLimit,
        DateTime? startsAt, DateTime? expiresAt, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            Fail("Mã voucher không được để trống.");
            return RedirectToAction(id == 0 ? nameof(Create) : nameof(Edit), id == 0 ? null : new { id });
        }

        var normalized = code.Trim().ToUpperInvariant();
        if (await Db.Vouchers.AnyAsync(v => v.Code == normalized && v.Id != id))
        {
            Fail($"Mã \"{normalized}\" đã tồn tại.");
            return RedirectToAction(id == 0 ? nameof(Create) : nameof(Edit), id == 0 ? null : new { id });
        }

        var v = id == 0 ? new Voucher { Code = "" } : await Db.Vouchers.FindAsync(id);
        if (v is null) return NotFound();

        v.Code = normalized;
        v.Label = label?.Trim() ?? "";
        v.Tag = string.IsNullOrWhiteSpace(tag) ? "Voucher" : tag.Trim();
        v.Type = type;
        v.Value = type == VoucherType.FreeShipping ? 0m : Math.Max(0, value);
        v.MinOrderAmount = Math.Max(0, minOrderAmount);
        v.MaxDiscountAmount = type == VoucherType.Percentage ? maxDiscountAmount : null;
        v.UsageLimit = usageLimit is > 0 ? usageLimit : null;
        v.StartsAt = startsAt;
        v.ExpiresAt = expiresAt;
        v.IsActive = isActive;

        if (id == 0) Db.Vouchers.Add(v);
        auth.Audit(id == 0 ? "Thêm voucher" : "Sửa voucher", nameof(Voucher), id == 0 ? null : id, normalized);
        await Db.SaveChangesAsync();
        Ok("Đã lưu voucher.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/voucher/{id:int}/an-hien")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var v = await Db.Vouchers.FindAsync(id);
        if (v is null) return NotFound();
        v.IsActive = !v.IsActive;
        auth.Audit(v.IsActive ? "Bật voucher" : "Tắt voucher", nameof(Voucher), id, v.Code);
        await Db.SaveChangesAsync();
        Ok(v.IsActive ? "Đã bật voucher." : "Đã tắt voucher.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/voucher/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var v = await Db.Vouchers.FindAsync(id);
        if (v is null) return NotFound();
        Db.Vouchers.Remove(v);
        auth.Audit("Xóa voucher", nameof(Voucher), id, v.Code);
        await Db.SaveChangesAsync();
        Ok("Đã xóa voucher.");
        return RedirectToAction(nameof(Index));
    }
}
