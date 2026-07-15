using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Services;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Areas.Admin.Controllers;

/// <summary>Edits the header menus (main + sub) and the footer columns/links. Every write drops
/// the NavigationService cache so the storefront updates immediately.</summary>
public class MenuController(HoaiiDbContext db, AdminAuthService auth, NavigationService nav) : BaseAdminController(db)
{
    [HttpGet("/admin/menu")]
    public async Task<IActionResult> Index()
    {
        ViewBag.Main = await Db.NavLinks.Where(l => l.Placement == NavPlacement.Main).OrderBy(l => l.SortOrder).ThenBy(l => l.Id).ToListAsync();
        ViewBag.Sub = await Db.NavLinks.Where(l => l.Placement == NavPlacement.Sub).OrderBy(l => l.SortOrder).ThenBy(l => l.Id).ToListAsync();
        ViewBag.Columns = await Db.FooterMenuColumns.Include(c => c.Links.OrderBy(l => l.SortOrder)).OrderBy(c => c.SortOrder).ThenBy(c => c.Id).ToListAsync();
        return View();
    }

    private void Done(string msg)
    {
        nav.Invalidate();
        Ok(msg);
    }

    // ---------- Header links ----------
    [HttpPost("/admin/menu/lien-ket/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LinkSave(int id, NavPlacement placement, string label, string url, bool hasDropdown, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(label) || string.IsNullOrWhiteSpace(url))
        {
            Fail("Nhãn và đường dẫn không được để trống.");
            return RedirectToAction(nameof(Index));
        }
        var x = id == 0 ? new NavLink { Label = "", Url = "" } : await Db.NavLinks.FindAsync(id);
        if (x is null) return NotFound();
        x.Placement = placement;
        x.Label = label.Trim();
        x.Url = url.Trim();
        x.HasDropdown = hasDropdown;
        x.SortOrder = sortOrder;
        if (id == 0) Db.NavLinks.Add(x);
        auth.Audit(id == 0 ? "Thêm liên kết menu" : "Sửa liên kết menu", nameof(NavLink), id == 0 ? null : id, label);
        await Db.SaveChangesAsync();
        Done("Đã lưu liên kết.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/menu/lien-ket/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LinkDelete(int id)
    {
        var x = await Db.NavLinks.FindAsync(id);
        if (x is null) return NotFound();
        Db.NavLinks.Remove(x);
        auth.Audit("Xóa liên kết menu", nameof(NavLink), id, x.Label);
        await Db.SaveChangesAsync();
        Done("Đã xóa liên kết.");
        return RedirectToAction(nameof(Index));
    }

    // ---------- Footer columns ----------
    [HttpPost("/admin/menu/cot/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ColumnSave(int id, string title, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            Fail("Tiêu đề cột không được để trống.");
            return RedirectToAction(nameof(Index));
        }
        var x = id == 0 ? new FooterMenuColumn { Title = "" } : await Db.FooterMenuColumns.FindAsync(id);
        if (x is null) return NotFound();
        x.Title = title.Trim();
        x.SortOrder = sortOrder;
        if (id == 0) Db.FooterMenuColumns.Add(x);
        auth.Audit(id == 0 ? "Thêm cột footer" : "Sửa cột footer", nameof(FooterMenuColumn), id == 0 ? null : id, title);
        await Db.SaveChangesAsync();
        Done("Đã lưu cột.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/menu/cot/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ColumnDelete(int id)
    {
        var x = await Db.FooterMenuColumns.FindAsync(id);
        if (x is null) return NotFound();
        Db.FooterMenuColumns.Remove(x); // links cascade
        auth.Audit("Xóa cột footer", nameof(FooterMenuColumn), id, x.Title);
        await Db.SaveChangesAsync();
        Done("Đã xóa cột.");
        return RedirectToAction(nameof(Index));
    }

    // ---------- Footer links ----------
    [HttpPost("/admin/menu/cot-lien-ket/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ColumnLinkSave(int id, int columnId, string label, string url, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(label) || string.IsNullOrWhiteSpace(url))
        {
            Fail("Nhãn và đường dẫn không được để trống.");
            return RedirectToAction(nameof(Index));
        }
        var x = id == 0 ? new FooterMenuLink { Label = "", Url = "", FooterMenuColumnId = columnId } : await Db.FooterMenuLinks.FindAsync(id);
        if (x is null) return NotFound();
        x.Label = label.Trim();
        x.Url = url.Trim();
        x.SortOrder = sortOrder;
        if (id == 0) Db.FooterMenuLinks.Add(x);
        auth.Audit(id == 0 ? "Thêm link footer" : "Sửa link footer", nameof(FooterMenuLink), id == 0 ? null : id, label);
        await Db.SaveChangesAsync();
        Done("Đã lưu liên kết footer.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/menu/cot-lien-ket/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ColumnLinkDelete(int id)
    {
        var x = await Db.FooterMenuLinks.FindAsync(id);
        if (x is null) return NotFound();
        Db.FooterMenuLinks.Remove(x);
        auth.Audit("Xóa link footer", nameof(FooterMenuLink), id, x.Label);
        await Db.SaveChangesAsync();
        Done("Đã xóa liên kết footer.");
        return RedirectToAction(nameof(Index));
    }
}
