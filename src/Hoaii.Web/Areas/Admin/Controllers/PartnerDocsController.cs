using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hoaii.Web.Areas.Admin.Filters;

namespace Hoaii.Web.Areas.Admin.Controllers;

/// <summary>
/// Tài liệu PDF cho đại lý / cộng tác viên: chính sách hoa hồng từng mùa, biểu mẫu đăng ký.
/// Tách khỏi Thư viện ảnh vì đây là tệp để khách TẢI VỀ, có tiêu đề và mùa vụ riêng.
/// </summary>
public class PartnerDocsController(HoaiiDbContext db, AdminAuthService auth, MediaService media)
    : BaseAdminController(db)
{
    [HttpGet("/admin/tai-lieu-dai-ly")]
    public async Task<IActionResult> Index()
    {
        var docs = await Db.PartnerDocuments
            .OrderBy(d => d.SortOrder).ThenByDescending(d => d.CreatedAt)
            .ToListAsync();
        return View(docs);
    }

    [HttpPost("/admin/tai-lieu-dai-ly/them")]
    [ValidateAntiForgeryToken]
    [GioiHanTep(MediaService.MaxDocBytes, MediaService.MaxDocLabel)]
    public async Task<IActionResult> Create(string title, string? season, int sortOrder, IFormFile? file)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            Fail("Tên tài liệu không được để trống.");
            return RedirectToAction(nameof(Index));
        }
        if (file is null || file.Length == 0)
        {
            Fail("Chưa chọn tệp PDF.");
            return RedirectToAction(nameof(Index));
        }

        var upload = await media.UploadDocumentAsync(file);
        if (!upload.Ok)
        {
            Fail(upload.Error ?? "Không tải được tệp.");
            return RedirectToAction(nameof(Index));
        }

        Db.PartnerDocuments.Add(new PartnerDocument
        {
            Title = title.Trim(),
            Season = string.IsNullOrWhiteSpace(season) ? null : season.Trim(),
            FileUrl = upload.Url!,
            FileName = upload.FileName ?? "tai-lieu.pdf",
            SizeBytes = upload.SizeBytes,
            SortOrder = sortOrder,
            CreatedAt = DateTime.UtcNow,
        });
        auth.Audit("Thêm tài liệu đại lý", nameof(PartnerDocument), null, title.Trim());
        await Db.SaveChangesAsync();
        Ok("Đã thêm tài liệu.");
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Ẩn/hiện thay vì xoá: mùa cũ thường cần giữ lại để tra cứu.</summary>
    [HttpPost("/admin/tai-lieu-dai-ly/{id:int}/an-hien")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int id)
    {
        var doc = await Db.PartnerDocuments.FindAsync(id);
        if (doc is null) return NotFound();
        doc.IsPublished = !doc.IsPublished;
        auth.Audit(doc.IsPublished ? "Hiện tài liệu đại lý" : "Ẩn tài liệu đại lý", nameof(PartnerDocument), id, doc.Title);
        await Db.SaveChangesAsync();
        Ok(doc.IsPublished ? "Đã hiện tài liệu." : "Đã ẩn tài liệu.");
        return RedirectToAction(nameof(Index));
    }

    // Xoá tài liệu là xoá luôn tệp trên đĩa, không hoàn tác được — cùng mức rủi ro với xoá sản
    // phẩm hay bài viết, nên cùng mức quyền: chỉ Chủ shop. Nhân viên vẫn ẩn được tài liệu.
    [HttpPost("/admin/tai-lieu-dai-ly/{id:int}/xoa")]
    [Authorize(Policy = AdminAuth.PolicyOwner)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var doc = await Db.PartnerDocuments.FindAsync(id);
        if (doc is null) return NotFound();

        media.DeleteFile(doc.FileUrl);
        Db.PartnerDocuments.Remove(doc);
        auth.Audit("Xóa tài liệu đại lý", nameof(PartnerDocument), id, doc.Title);
        await Db.SaveChangesAsync();
        Ok("Đã xóa tài liệu.");
        return RedirectToAction(nameof(Index));
    }
}
