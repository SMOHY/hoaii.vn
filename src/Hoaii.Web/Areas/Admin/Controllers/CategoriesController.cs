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
    public async Task<IActionResult> Create()
    {
        ViewBag.Types = TypeOptions();
        ViewBag.Groups = await Db.CategoryGroups.OrderBy(g => g.SortOrder).ToListAsync();
        return View("Edit", new Category { Name = "", Slug = "" });
    }

    [HttpGet("/admin/danh-muc/{id:int}/sua")]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await Db.Categories.FindAsync(id);
        if (category is null) return NotFound();
        ViewBag.Types = TypeOptions();
        ViewBag.Groups = await Db.CategoryGroups.OrderBy(g => g.SortOrder).ToListAsync();
        ViewBag.HeroSlides = await Db.CategoryHeroSlides
            .Where(s => s.CategoryId == id)
            .OrderBy(s => s.SortOrder).ThenBy(s => s.Id)
            .ToListAsync();
        return View(category);
    }

    [HttpPost("/admin/danh-muc/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(int id, string name, string? slug, CategoryType type, int sortOrder, int? groupId, CategoryCms cms)
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
            var category = new Category { Name = name.Trim(), Slug = finalSlug, Type = type, SortOrder = sortOrder, GroupId = groupId };
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
            category.GroupId = groupId;
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
        public string? BannerImageUrlMobile { get; set; }
        public string? BannerImageFocal { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? CoverImageUrlMobile { get; set; }
        public string? CoverImageFocal { get; set; }
        public string? PromoEyebrow { get; set; }
        public string? PromoTitle { get; set; }
        public string? PromoCtaText { get; set; }
        public string? PromoCtaUrl { get; set; }
        public string? PromoImageUrl { get; set; }
        public string? PromoImageUrlMobile { get; set; }
        public string? PromoImageFocal { get; set; }
        public string? PromoBackground { get; set; }
        public bool PromoWide { get; set; }
    }

    private static void ApplyCms(Category c, CategoryCms cms)
    {
        c.Description = Clean(cms.Description);
        c.HeroEyebrow = Clean(cms.HeroEyebrow);
        c.HeroKicker = Clean(cms.HeroKicker);
        c.BannerImageUrl = Clean(cms.BannerImageUrl);
        c.BannerImageUrlMobile = Clean(cms.BannerImageUrlMobile);
        c.BannerImageFocal = Focal(cms.BannerImageFocal);
        c.CoverImageUrl = Clean(cms.CoverImageUrl);
        c.CoverImageUrlMobile = Clean(cms.CoverImageUrlMobile);
        c.CoverImageFocal = Focal(cms.CoverImageFocal);
        c.PromoEyebrow = Clean(cms.PromoEyebrow);
        c.PromoTitle = Clean(cms.PromoTitle);
        c.PromoCtaText = Clean(cms.PromoCtaText);
        c.PromoCtaUrl = Clean(cms.PromoCtaUrl);
        c.PromoImageUrl = Clean(cms.PromoImageUrl);
        c.PromoImageUrlMobile = Clean(cms.PromoImageUrlMobile);
        c.PromoImageFocal = Focal(cms.PromoImageFocal);
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

    // ---------- Hero slide (per-category, see CategoryController.Index) ----------
    [HttpGet("/admin/danh-muc/{categoryId:int}/slide/them")]
    public async Task<IActionResult> HeroSlideCreate(int categoryId)
    {
        var category = await Db.Categories.FindAsync(categoryId);
        if (category is null) return NotFound();
        ViewBag.Category = category;
        return View("HeroSlideEdit", new CategoryHeroSlide { CategoryId = categoryId, ImageUrl = "" });
    }

    [HttpGet("/admin/danh-muc/slide/{id:int}/sua")]
    public async Task<IActionResult> HeroSlideEdit(int id)
    {
        var slide = await Db.CategoryHeroSlides.FindAsync(id);
        if (slide is null) return NotFound();
        ViewBag.Category = await Db.Categories.FindAsync(slide.CategoryId);
        return View(slide);
    }

    [HttpPost("/admin/danh-muc/slide/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HeroSlideSave(int id, int categoryId, string imageUrl, string? mobileImageUrl, string? imageFocal, string? name, string? linkUrl, int sortOrder, bool isActive)
    {
        if (await Db.Categories.FindAsync(categoryId) is null) return NotFound();

        var slide = id == 0 ? new CategoryHeroSlide { CategoryId = categoryId, ImageUrl = "" } : await Db.CategoryHeroSlides.FindAsync(id);
        if (slide is null) return NotFound();
        slide.CategoryId = categoryId;
        slide.ImageUrl = imageUrl?.Trim() ?? "";
        slide.MobileImageUrl = Clean(mobileImageUrl);
        slide.ImageFocal = Focal(imageFocal);
        slide.Name = name?.Trim() ?? "";
        slide.LinkUrl = string.IsNullOrWhiteSpace(linkUrl) ? "#" : linkUrl.Trim();
        slide.SortOrder = sortOrder;
        slide.IsActive = isActive;
        if (id == 0) Db.CategoryHeroSlides.Add(slide);
        auth.Audit(id == 0 ? "Thêm slide hero danh mục" : "Sửa slide hero danh mục", nameof(CategoryHeroSlide), id == 0 ? null : id);
        await Db.SaveChangesAsync();
        Ok("Đã lưu slide.");
        return RedirectToAction(nameof(Edit), new { id = categoryId });
    }

    [HttpPost("/admin/danh-muc/slide/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HeroSlideDelete(int id)
    {
        var slide = await Db.CategoryHeroSlides.FindAsync(id);
        if (slide is null) return NotFound();
        var categoryId = slide.CategoryId;
        Db.CategoryHeroSlides.Remove(slide);
        auth.Audit("Xóa slide hero danh mục", nameof(CategoryHeroSlide), id);
        await Db.SaveChangesAsync();
        Ok("Đã xóa slide.");
        return RedirectToAction(nameof(Edit), new { id = categoryId });
    }

    private static List<SelectListItem> TypeOptions() =>
    [
        new("Loại sản phẩm (Trà, Khăn…)", nameof(CategoryType.ProductType)),
        new("Theo dịp (Quà tết, Trung thu…)", nameof(CategoryType.Occasion)),
    ];
}
