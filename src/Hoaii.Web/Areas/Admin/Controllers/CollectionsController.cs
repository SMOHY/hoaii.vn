using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Areas.Admin.Controllers;

/// <summary>CRUD for Collection — named product lines that span categories (e.g. "Thiên Điểu Lạc
/// Hồng" as both a Tết box and a Trung Thu mooncake box). Products pick one from
/// Areas/Admin/Views/Products/Edit.cshtml; the mega-menu's "Theo bộ sưu tập" column reads it.</summary>
public class CollectionsController(HoaiiDbContext db, AdminAuthService auth) : BaseAdminController(db)
{
    [HttpGet("/admin/bo-suu-tap")]
    public async Task<IActionResult> Index()
    {
        var collections = await Db.Collections
            .OrderBy(c => c.SortOrder).ThenBy(c => c.Id)
            .Select(c => new { c.Id, c.Name, c.SortOrder, ProductCount = Db.Products.Count(p => p.CollectionId == c.Id) })
            .ToListAsync();
        return View(collections.Select(c => (c.Id, c.Name, c.SortOrder, c.ProductCount)).ToList());
    }

    [HttpPost("/admin/bo-suu-tap/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(int id, string name, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            Fail("Tên bộ sưu tập không được để trống.");
            return RedirectToAction(nameof(Index));
        }
        var x = id == 0 ? new Collection { Name = "" } : await Db.Collections.FindAsync(id);
        if (x is null) return NotFound();
        x.Name = name.Trim();
        x.SortOrder = sortOrder;
        if (id == 0) Db.Collections.Add(x);
        auth.Audit(id == 0 ? "Thêm bộ sưu tập" : "Sửa bộ sưu tập", nameof(Collection), id == 0 ? null : id, name);
        await Db.SaveChangesAsync();
        Ok("Đã lưu bộ sưu tập.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/bo-suu-tap/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var x = await Db.Collections.FindAsync(id);
        if (x is null) return NotFound();
        // FK is SetNull, so products in this collection just lose the tag rather than blocking the delete.
        Db.Collections.Remove(x);
        auth.Audit("Xóa bộ sưu tập", nameof(Collection), id, x.Name);
        await Db.SaveChangesAsync();
        Ok("Đã xóa bộ sưu tập.");
        return RedirectToAction(nameof(Index));
    }
}
