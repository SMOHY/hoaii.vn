using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Services;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Areas.Admin.Controllers;

public partial class CategoriesController(HoaiiDbContext db, AdminAuthService auth) : BaseAdminController(db)
{
    [System.Text.RegularExpressions.GeneratedRegex(@"^#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6})$")]
    private static partial System.Text.RegularExpressions.Regex HexColour();

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
    public async Task<IActionResult> Save(int id, string name, string? slug, CategoryType type, int sortOrder, CategoryCms cms)
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
            ApplyCms(category, cms);
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
            ApplyCms(category, cms);
            auth.Audit("Sửa danh mục", nameof(Category), id, name);
            await Db.SaveChangesAsync();
            Ok("Đã lưu danh mục.");
        }
        return RedirectToAction(nameof(Index));
    }

    /// <summary>CMS copy for the category landing page, model-bound as cms.Description etc.</summary>
    public class CategoryCms
    {
        public string? Description { get; set; }
        public string? HeroEyebrow { get; set; }
        public string? HeroKicker { get; set; }
        public string? BannerImageUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? PromoEyebrow { get; set; }
        public string? PromoTitle { get; set; }
        public string? PromoCtaText { get; set; }
        public string? PromoCtaUrl { get; set; }
        public string? PromoImageUrl { get; set; }
        public string? PromoBackground { get; set; }
        public bool PromoWide { get; set; }
    }

    private static string? Clean(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

    private static void ApplyCms(Category c, CategoryCms cms)
    {
        c.Description = Clean(cms.Description);
        c.HeroEyebrow = Clean(cms.HeroEyebrow);
        c.HeroKicker = Clean(cms.HeroKicker);
        c.BannerImageUrl = Clean(cms.BannerImageUrl);
        c.CoverImageUrl = Clean(cms.CoverImageUrl);
        c.PromoEyebrow = Clean(cms.PromoEyebrow);
        c.PromoTitle = Clean(cms.PromoTitle);
        c.PromoCtaText = Clean(cms.PromoCtaText);
        c.PromoCtaUrl = Clean(cms.PromoCtaUrl);
        c.PromoImageUrl = Clean(cms.PromoImageUrl);
        // Chỉ nhận mã hex. Giá trị này được ghi thẳng vào thuộc tính style của dải campaign, nên
        // một chuỗi kiểu "red;background-image:url(...)" sẽ chèn được CSS lạ vào trang.
        var bg = Clean(cms.PromoBackground);
        c.PromoBackground = bg is not null && HexColour().IsMatch(bg) ? bg : null;
        c.PromoWide = cms.PromoWide;
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
