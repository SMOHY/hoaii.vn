using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Areas.Admin.Controllers;

/// <summary>Rename the 2 existing "Quà theo dịp" chooser groups (see CategoryGroup) — adding a
/// brand-new group isn't supported here because a new group still needs a matching landing page
/// built in code (OccasionController.Pages), which is a bigger feature deferred for now.</summary>
public class CategoryGroupsController(HoaiiDbContext db, AdminAuthService auth) : BaseAdminController(db)
{
    [HttpPost("/admin/nhom-danh-muc/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(int id, string name, int sortOrder)
    {
        var x = await Db.CategoryGroups.FindAsync(id);
        if (x is null || string.IsNullOrWhiteSpace(name))
        {
            Fail("Không lưu được — kiểm tra lại tên nhóm.");
            return RedirectToAction("Index", "Menu");
        }
        x.Name = name.Trim();
        x.SortOrder = sortOrder;
        auth.Audit("Sửa nhóm danh mục", nameof(CategoryGroup), id, name);
        await Db.SaveChangesAsync();
        Ok("Đã lưu nhóm.");
        return RedirectToAction("Index", "Menu");
    }

    /// <summary>Move one Occasion category into a group (or out, if groupId is null) — this is
    /// what "+ Thêm danh mục" / "✕" on a group's chip list in the Menu editor calls.</summary>
    [HttpPost("/admin/nhom-danh-muc/gan")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignCategory(int categoryId, int? groupId)
    {
        var category = await Db.Categories.FindAsync(categoryId);
        if (category is null) return NotFound();
        category.GroupId = groupId;
        auth.Audit("Gán danh mục vào nhóm", nameof(Category), categoryId, category.Name);
        await Db.SaveChangesAsync();
        Ok("Đã lưu.");
        return RedirectToAction("Index", "Menu");
    }
}
