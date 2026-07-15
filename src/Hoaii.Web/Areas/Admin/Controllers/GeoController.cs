using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Areas.Admin.Controllers;

/// <summary>
/// Manages the administrative divisions (provinces + wards) used by the customer address book.
/// The seed only ships a sample; this screen lets the shop load the full official list, including
/// a bulk paste import so thousands of wards don't have to be typed one by one.
/// </summary>
public class GeoController(HoaiiDbContext db, AdminAuthService auth) : BaseAdminController(db)
{
    [HttpGet("/admin/dia-gioi")]
    public async Task<IActionResult> Index()
    {
        var provinces = await Db.Provinces
            .OrderBy(p => p.Name)
            .Select(p => new { p.Id, p.Name, WardCount = p.Wards.Count })
            .ToListAsync();
        return View(provinces.Select(p => (p.Id, p.Name, p.WardCount)).ToList());
    }

    [HttpGet("/admin/dia-gioi/tinh/{id:int}")]
    public async Task<IActionResult> Province(int id)
    {
        var province = await Db.Provinces
            .Include(p => p.Wards.OrderBy(w => w.Name))
            .FirstOrDefaultAsync(p => p.Id == id);
        return province is null ? NotFound() : View(province);
    }

    [HttpPost("/admin/dia-gioi/tinh/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProvinceSave(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            Fail("Tên tỉnh/thành không được để trống.");
            return RedirectToAction(nameof(Index));
        }
        var trimmed = name.Trim();
        if (id == 0)
        {
            if (!await Db.Provinces.AnyAsync(p => p.Name == trimmed))
            {
                Db.Provinces.Add(new Province { Name = trimmed });
                auth.Audit("Thêm tỉnh/thành", nameof(Province), null, trimmed);
                await Db.SaveChangesAsync();
            }
            Ok("Đã thêm tỉnh/thành.");
        }
        else
        {
            var p = await Db.Provinces.FindAsync(id);
            if (p is null) return NotFound();
            p.Name = trimmed;
            auth.Audit("Sửa tỉnh/thành", nameof(Province), id, trimmed);
            await Db.SaveChangesAsync();
            Ok("Đã lưu tỉnh/thành.");
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/dia-gioi/tinh/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProvinceDelete(int id)
    {
        var province = await Db.Provinces.Include(p => p.Wards).FirstOrDefaultAsync(p => p.Id == id);
        if (province is null) return NotFound();

        var wardIds = province.Wards.Select(w => w.Id).ToList();
        if (await Db.Addresses.AnyAsync(a => a.ProvinceId == id || wardIds.Contains(a.WardId)))
        {
            Fail("Không thể xóa: có địa chỉ khách hàng đang dùng tỉnh/phường này.");
            return RedirectToAction(nameof(Index));
        }

        Db.Wards.RemoveRange(province.Wards);
        Db.Provinces.Remove(province);
        auth.Audit("Xóa tỉnh/thành", nameof(Province), id, province.Name);
        await Db.SaveChangesAsync();
        Ok("Đã xóa tỉnh/thành.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/dia-gioi/phuong/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> WardSave(int id, int provinceId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            Fail("Tên phường/xã không được để trống.");
            return RedirectToAction(nameof(Province), new { id = provinceId });
        }
        var trimmed = name.Trim();
        if (id == 0)
        {
            if (!await Db.Wards.AnyAsync(w => w.ProvinceId == provinceId && w.Name == trimmed))
            {
                Db.Wards.Add(new Ward { ProvinceId = provinceId, Name = trimmed });
                await Db.SaveChangesAsync();
            }
            Ok("Đã thêm phường/xã.");
        }
        else
        {
            var w = await Db.Wards.FindAsync(id);
            if (w is null) return NotFound();
            w.Name = trimmed;
            await Db.SaveChangesAsync();
            Ok("Đã lưu phường/xã.");
        }
        return RedirectToAction(nameof(Province), new { id = provinceId });
    }

    [HttpPost("/admin/dia-gioi/phuong/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> WardDelete(int id)
    {
        var ward = await Db.Wards.FindAsync(id);
        if (ward is null) return NotFound();
        if (await Db.Addresses.AnyAsync(a => a.WardId == id))
        {
            Fail("Không thể xóa: có địa chỉ khách hàng đang dùng phường/xã này.");
            return RedirectToAction(nameof(Province), new { id = ward.ProvinceId });
        }
        var provinceId = ward.ProvinceId;
        Db.Wards.Remove(ward);
        await Db.SaveChangesAsync();
        Ok("Đã xóa phường/xã.");
        return RedirectToAction(nameof(Province), new { id = provinceId });
    }

    /// <summary>Bulk import: one "Tỉnh | Phường" per line (or "Tỉnh" alone). Creates provinces and
    /// wards that don't exist yet; skips duplicates. Lets the shop paste the full official list.</summary>
    [HttpPost("/admin/dia-gioi/import")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Import(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            Fail("Chưa có dữ liệu để nhập.");
            return RedirectToAction(nameof(Index));
        }

        var provinces = await Db.Provinces.Include(p => p.Wards).ToListAsync();
        var byName = provinces.ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
        int newProvinces = 0, newWards = 0;

        foreach (var raw in data.Replace("\r\n", "\n").Split('\n'))
        {
            var line = raw.Trim();
            if (line.Length == 0) continue;

            var parts = line.Split('|', 2);
            var provinceName = parts[0].Trim();
            if (provinceName.Length == 0) continue;

            if (!byName.TryGetValue(provinceName, out var province))
            {
                province = new Province { Name = provinceName };
                Db.Provinces.Add(province);
                byName[provinceName] = province;
                newProvinces++;
            }

            if (parts.Length == 2)
            {
                var wardName = parts[1].Trim();
                if (wardName.Length > 0 && !province.Wards.Any(w => w.Name.Equals(wardName, StringComparison.OrdinalIgnoreCase)))
                {
                    province.Wards.Add(new Ward { Name = wardName });
                    newWards++;
                }
            }
        }

        await Db.SaveChangesAsync();
        auth.Audit("Nhập địa giới", nameof(Province), null, $"+{newProvinces} tỉnh, +{newWards} phường");
        await Db.SaveChangesAsync();
        Ok($"Đã nhập: thêm {newProvinces} tỉnh/thành, {newWards} phường/xã.");
        return RedirectToAction(nameof(Index));
    }
}
