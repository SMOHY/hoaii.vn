using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Services;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Areas.Admin.Controllers;

public class PoliciesController(HoaiiDbContext db, AdminAuthService auth) : BaseAdminController(db)
{
    [HttpGet("/admin/chinh-sach")]
    public async Task<IActionResult> Index()
    {
        var pages = await Db.PolicyPages
            .OrderBy(p => p.SortOrder).ThenBy(p => p.Id)
            .Select(p => new { p.Id, p.Title, p.Slug, p.IsPublished, BlockCount = p.Blocks.Count })
            .ToListAsync();
        return View(pages.Select(p => (p.Id, p.Title, p.Slug, p.IsPublished, p.BlockCount)).ToList());
    }

    [HttpGet("/admin/chinh-sach/them")]
    public IActionResult Create() =>
        View("Edit", new PolicyPage { Slug = "", Title = "", NavLabel = "", BreadcrumbLabel = "" });

    [HttpGet("/admin/chinh-sach/{id:int}/sua")]
    public async Task<IActionResult> Edit(int id)
    {
        var page = await Db.PolicyPages
            .Include(p => p.Blocks.OrderBy(b => b.SortOrder))
            .FirstOrDefaultAsync(p => p.Id == id);
        if (page is null) return NotFound();
        return View(page);
    }

    [HttpPost("/admin/chinh-sach/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(
        int id, string title, string? slug, string navLabel, string? breadcrumbLabel,
        int sortOrder, bool isPublished,
        List<PolicyBlockKind>? blockKinds, List<string>? blockTexts)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(navLabel))
        {
            Fail("Tiêu đề và nhãn không được để trống.");
            return RedirectToAction(id == 0 ? nameof(Create) : nameof(Edit), id == 0 ? null : new { id });
        }

        var finalSlug = string.IsNullOrWhiteSpace(slug) ? Slug.From(title) : Slug.From(slug);
        if (await Db.PolicyPages.AnyAsync(p => p.Slug == finalSlug && p.Id != id))
        {
            Fail($"Slug \"{finalSlug}\" đã tồn tại.");
            return RedirectToAction(id == 0 ? nameof(Create) : nameof(Edit), id == 0 ? null : new { id });
        }

        var blocks = BuildBlocks(blockKinds, blockTexts);
        var crumb = string.IsNullOrWhiteSpace(breadcrumbLabel) ? $"Trang chủ/{navLabel.Trim()}" : breadcrumbLabel.Trim();

        if (id == 0)
        {
            var page = new PolicyPage
            {
                Slug = finalSlug,
                Title = title.Trim(),
                NavLabel = navLabel.Trim(),
                BreadcrumbLabel = crumb,
                SortOrder = sortOrder,
                IsPublished = isPublished,
                Blocks = blocks,
            };
            Db.PolicyPages.Add(page);
            auth.Audit("Thêm trang chính sách", nameof(PolicyPage), null, title);
            await Db.SaveChangesAsync();
            Ok("Đã thêm trang chính sách.");
        }
        else
        {
            var page = await Db.PolicyPages
                .Include(p => p.Blocks)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (page is null) return NotFound();

            page.Slug = finalSlug;
            page.Title = title.Trim();
            page.NavLabel = navLabel.Trim();
            page.BreadcrumbLabel = crumb;
            page.SortOrder = sortOrder;
            page.IsPublished = isPublished;

            // Replace the whole block list — simplest correct behaviour for a reorderable editor.
            Db.PolicyBlocks.RemoveRange(page.Blocks);
            page.Blocks = blocks;

            auth.Audit("Sửa trang chính sách", nameof(PolicyPage), id, title);
            await Db.SaveChangesAsync();
            Ok("Đã lưu trang chính sách.");
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/chinh-sach/{id:int}/xoa")]
    [Authorize(Policy = AdminAuth.PolicyOwner)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var page = await Db.PolicyPages.FindAsync(id);
        if (page is null) return NotFound();
        Db.PolicyPages.Remove(page); // blocks cascade
        auth.Audit("Xóa trang chính sách", nameof(PolicyPage), id, page.Title);
        await Db.SaveChangesAsync();
        Ok("Đã xóa trang chính sách.");
        return RedirectToAction(nameof(Index));
    }

    private static List<PolicyBlock> BuildBlocks(List<PolicyBlockKind>? kinds, List<string>? texts)
    {
        var blocks = new List<PolicyBlock>();
        if (kinds is null || texts is null) return blocks;

        var count = Math.Min(kinds.Count, texts.Count);
        var order = 0;
        for (var i = 0; i < count; i++)
        {
            var text = texts[i]?.Trim();
            if (string.IsNullOrWhiteSpace(text)) continue; // drop empty rows
            blocks.Add(new PolicyBlock { Kind = kinds[i], Text = text, SortOrder = order++ });
        }
        return blocks;
    }
}
