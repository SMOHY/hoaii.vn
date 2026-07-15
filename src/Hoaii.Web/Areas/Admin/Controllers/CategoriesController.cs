using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Services;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Areas.Admin.Controllers;

public class CategoriesController(HoaiiDbContext db, AdminAuthService auth) : BaseAdminController(db)
{
    [HttpGet("/admin/danh-muc")]
    public async Task<IActionResult> Index()
    {
        var categories = await Db.Categories
            .OrderBy(c => c.Type).ThenBy(c => c.SortOrder).ThenBy(c => c.Id)
            .Select(c => new { c.Id, c.Name, c.Slug, c.Type, c.SortOrder, ProductCount = c.Products.Count })
            .ToListAsync();
        return View(categories.Select(c => (c.Id, c.Name, c.Slug, c.Type, c.SortOrder, c.ProductCount)).ToList());
    }

    [HttpGet("/admin/danh-muc/them")]
    public IActionResult Create()
    {
        ViewBag.Types = TypeOptions();
        return View("Edit", new Category { Name = "", Slug = "" });
    }

    [HttpGet("/admin/danh-muc/{id:int}/sua")]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await Db.Categories.FindAsync(id);
        if (category is null) return NotFound();
        ViewBag.Types = TypeOptions();
        return View(category);
    }

    [HttpPost("/admin/danh-muc/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(int id, string name, string? slug, CategoryType type, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            Fail("Tên danh mục không được để trống.");
            return RedirectToAction(id == 0 ? nameof(Create) : nameof(Edit), id == 0 ? null : new { id });
        }

        var finalSlug = string.IsNullOrWhiteSpace(slug) ? Slug.From(name) : Slug.From(slug);

        // Slug must stay unique — the storefront routes /danh-muc/{slug} straight to it.
        if (await Db.Categories.AnyAsync(c => c.Slug == finalSlug && c.Id != id))
        {
            Fail($"Slug \"{finalSlug}\" đã tồn tại.");
            return RedirectToAction(id == 0 ? nameof(Create) : nameof(Edit), id == 0 ? null : new { id });
        }

        if (id == 0)
        {
            var category = new Category { Name = name.Trim(), Slug = finalSlug, Type = type, SortOrder = sortOrder };
            Db.Categories.Add(category);
            auth.Audit("Thêm danh mục", nameof(Category), null, name);
            await Db.SaveChangesAsync();
            Ok("Đã thêm danh mục.");
        }
        else
        {
            var category = await Db.Categories.FindAsync(id);
            if (category is null) return NotFound();
            category.Name = name.Trim();
            category.Slug = finalSlug;
            category.Type = type;
            category.SortOrder = sortOrder;
            auth.Audit("Sửa danh mục", nameof(Category), id, name);
            await Db.SaveChangesAsync();
            Ok("Đã lưu danh mục.");
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/danh-muc/{id:int}/xoa")]
    [Microsoft.AspNetCore.Authorization.Authorize(Policy = AdminAuth.PolicyOwner)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await Db.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
        if (category is null) return NotFound();

        // The FK is Restrict — deleting a category with products would throw. Block it clearly.
        if (category.Products.Count > 0)
        {
            Fail($"Không thể xóa: danh mục còn {category.Products.Count} sản phẩm. Hãy chuyển sản phẩm sang danh mục khác trước.");
            return RedirectToAction(nameof(Index));
        }

        Db.Categories.Remove(category);
        auth.Audit("Xóa danh mục", nameof(Category), id, category.Name);
        await Db.SaveChangesAsync();
        Ok("Đã xóa danh mục.");
        return RedirectToAction(nameof(Index));
    }

    private static List<SelectListItem> TypeOptions() =>
    [
        new("Loại sản phẩm (Trà, Khăn…)", nameof(CategoryType.ProductType)),
        new("Theo dịp (Quà tết, Trung thu…)", nameof(CategoryType.Occasion)),
    ];
}
