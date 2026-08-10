using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Services;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Areas.Admin.Controllers;

/// <summary>CRUD for Collection — named product lines that span categories (e.g. "Thiên Điểu Lạc
/// Hồng" as both a Tết box and a Trung Thu mooncake box). Products pick one from
/// Areas/Admin/Views/Products/Edit.cshtml; the mega-menu's "Theo bộ sưu tập" column reads it. Each
/// collection also has its own public landing page (CollectionController, /bo-suu-tap/{slug}), so
/// this controller mirrors CategoriesController's shape — a list plus a per-collection Edit page
/// with CMS fields and a hero slide manager.</summary>
public partial class CollectionsController(HoaiiDbContext db, AdminAuthService auth) : BaseAdminController(db)
{
    [System.Text.RegularExpressions.GeneratedRegex(@"^#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6})$")]
    private static partial System.Text.RegularExpressions.Regex HexColour();

    [HttpGet("/admin/bo-suu-tap")]
    public async Task<IActionResult> Index()
    {
        var collections = await Db.Collections
            .OrderBy(c => c.SortOrder).ThenBy(c => c.Id)
            .Select(c => new { c.Id, c.Name, c.Slug, c.SortOrder, ProductCount = Db.Products.Count(p => p.CollectionId == c.Id) })
            .ToListAsync();
        return View(collections.Select(c => (c.Id, c.Name, c.Slug, c.SortOrder, c.ProductCount)).ToList());
    }

    [HttpGet("/admin/bo-suu-tap/them")]
    public IActionResult Create() => View("Edit", new Collection { Name = "", Slug = "" });

    [HttpGet("/admin/bo-suu-tap/{id:int}/sua")]
    public async Task<IActionResult> Edit(int id)
    {
        var collection = await Db.Collections.FindAsync(id);
        if (collection is null) return NotFound();
        ViewBag.HeroSlides = await Db.CollectionHeroSlides
            .Where(s => s.CollectionId == id)
            .OrderBy(s => s.SortOrder).ThenBy(s => s.Id)
            .ToListAsync();
        return View(collection);
    }

    [HttpPost("/admin/bo-suu-tap/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(int id, string name, string? slug, int sortOrder, CollectionCms cms)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            Fail("Tên bộ sưu tập không được để trống.");
            return RedirectToAction(id == 0 ? nameof(Create) : nameof(Edit), id == 0 ? null : new { id });
        }

        var finalSlug = string.IsNullOrWhiteSpace(slug) ? Slug.From(name) : Slug.From(slug);

        // Slug must stay unique — the storefront routes /bo-suu-tap/{slug} straight to it.
        if (await Db.Collections.AnyAsync(c => c.Slug == finalSlug && c.Id != id))
        {
            Fail($"Slug \"{finalSlug}\" đã tồn tại.");
            return RedirectToAction(id == 0 ? nameof(Create) : nameof(Edit), id == 0 ? null : new { id });
        }

        if (id == 0)
        {
            var collection = new Collection { Name = name.Trim(), Slug = finalSlug, SortOrder = sortOrder };
            ApplyCms(collection, cms);
            Db.Collections.Add(collection);
            auth.Audit("Thêm bộ sưu tập", nameof(Collection), null, name);
            await Db.SaveChangesAsync();
            Ok("Đã thêm bộ sưu tập.");
        }
        else
        {
            var collection = await Db.Collections.FindAsync(id);
            if (collection is null) return NotFound();
            collection.Name = name.Trim();
            collection.Slug = finalSlug;
            collection.SortOrder = sortOrder;
            ApplyCms(collection, cms);
            auth.Audit("Sửa bộ sưu tập", nameof(Collection), id, name);
            await Db.SaveChangesAsync();
            Ok("Đã lưu bộ sưu tập.");
        }
        return RedirectToAction(nameof(Index));
    }

    /// <summary>CMS copy for the collection landing page, model-bound as cms.Description etc.</summary>
    public class CollectionCms
    {
        public string? Description { get; set; }
        public string? HeroEyebrow { get; set; }
        public string? HeroKicker { get; set; }
        public string? PromoEyebrow { get; set; }
        public string? PromoTitle { get; set; }
        public string? PromoCtaText { get; set; }
        public string? PromoCtaUrl { get; set; }
        public string? PromoImageUrl { get; set; }
        public string? PromoBackground { get; set; }
        public bool PromoWide { get; set; }
    }

    private static string? Clean(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

    private static void ApplyCms(Collection c, CollectionCms cms)
    {
        c.Description = Clean(cms.Description);
        c.HeroEyebrow = Clean(cms.HeroEyebrow);
        c.HeroKicker = Clean(cms.HeroKicker);
        c.PromoEyebrow = Clean(cms.PromoEyebrow);
        c.PromoTitle = Clean(cms.PromoTitle);
        c.PromoCtaText = Clean(cms.PromoCtaText);
        c.PromoCtaUrl = Clean(cms.PromoCtaUrl);
        c.PromoImageUrl = Clean(cms.PromoImageUrl);
        // Chỉ nhận mã hex — xem CategoriesController.ApplyCms cho lý do (giá trị này ghi thẳng
        // vào style attribute của dải campaign).
        var bg = Clean(cms.PromoBackground);
        c.PromoBackground = bg is not null && HexColour().IsMatch(bg) ? bg : null;
        c.PromoWide = cms.PromoWide;
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

    // ---------- Hero slide (per-collection, see CollectionController.Index) ----------
    [HttpGet("/admin/bo-suu-tap/{collectionId:int}/slide/them")]
    public async Task<IActionResult> HeroSlideCreate(int collectionId)
    {
        var collection = await Db.Collections.FindAsync(collectionId);
        if (collection is null) return NotFound();
        ViewBag.Collection = collection;
        return View("HeroSlideEdit", new CollectionHeroSlide { CollectionId = collectionId, ImageUrl = "" });
    }

    [HttpGet("/admin/bo-suu-tap/slide/{id:int}/sua")]
    public async Task<IActionResult> HeroSlideEdit(int id)
    {
        var slide = await Db.CollectionHeroSlides.FindAsync(id);
        if (slide is null) return NotFound();
        ViewBag.Collection = await Db.Collections.FindAsync(slide.CollectionId);
        return View(slide);
    }

    [HttpPost("/admin/bo-suu-tap/slide/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HeroSlideSave(int id, int collectionId, string imageUrl, string? name, string? linkUrl, int sortOrder, bool isActive)
    {
        if (await Db.Collections.FindAsync(collectionId) is null) return NotFound();

        var slide = id == 0 ? new CollectionHeroSlide { CollectionId = collectionId, ImageUrl = "" } : await Db.CollectionHeroSlides.FindAsync(id);
        if (slide is null) return NotFound();
        slide.CollectionId = collectionId;
        slide.ImageUrl = imageUrl?.Trim() ?? "";
        slide.Name = name?.Trim() ?? "";
        slide.LinkUrl = string.IsNullOrWhiteSpace(linkUrl) ? "#" : linkUrl.Trim();
        slide.SortOrder = sortOrder;
        slide.IsActive = isActive;
        if (id == 0) Db.CollectionHeroSlides.Add(slide);
        auth.Audit(id == 0 ? "Thêm slide hero bộ sưu tập" : "Sửa slide hero bộ sưu tập", nameof(CollectionHeroSlide), id == 0 ? null : id);
        await Db.SaveChangesAsync();
        Ok("Đã lưu slide.");
        return RedirectToAction(nameof(Edit), new { id = collectionId });
    }

    [HttpPost("/admin/bo-suu-tap/slide/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HeroSlideDelete(int id)
    {
        var slide = await Db.CollectionHeroSlides.FindAsync(id);
        if (slide is null) return NotFound();
        var collectionId = slide.CollectionId;
        Db.CollectionHeroSlides.Remove(slide);
        auth.Audit("Xóa slide hero bộ sưu tập", nameof(CollectionHeroSlide), id);
        await Db.SaveChangesAsync();
        Ok("Đã xóa slide.");
        return RedirectToAction(nameof(Edit), new { id = collectionId });
    }
}
