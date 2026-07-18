using Hoaii.Infrastructure;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Areas.Admin.Controllers;

public class MediaController(HoaiiDbContext db, MediaService media) : BaseAdminController(db)
{
    [HttpGet("/admin/thu-vien-anh")]
    public async Task<IActionResult> Index()
    {
        var assets = await Db.MediaAssets.OrderByDescending(m => m.CreatedAt).Take(200).ToListAsync();
        return View(assets);
    }

    // A touch above MediaService.MaxBytes (5MB) so a slightly-oversized file still reaches the
    // service and gets the friendly "vượt quá 5MB" message rather than a bare framework 413.
    [HttpPost("/admin/thu-vien-anh/tai-len")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> Upload(List<IFormFile> files, bool json = false)
    {
        var uploaded = new List<object>();
        var errors = new List<string>();

        foreach (var file in files)
        {
            var result = await media.UploadAsync(file);
            if (result.Ok && result.Asset is not null)
            {
                uploaded.Add(new { id = result.Asset.Id, url = result.Asset.Url, name = result.Asset.FileName });
            }
            else
            {
                errors.Add($"{file.FileName}: {result.Error}");
            }
        }

        // The picker modal uploads over fetch and wants JSON back; the standalone library page
        // posts a normal form and wants a redirect with a flash message.
        if (json)
        {
            return Ok(new { uploaded, errors });
        }

        if (errors.Count > 0)
        {
            Fail(string.Join(" · ", errors));
        }
        if (uploaded.Count > 0)
        {
            Ok($"Đã tải lên {uploaded.Count} ảnh.");
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/thu-vien-anh/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await media.DeleteAsync(id);
        Ok("Đã xóa ảnh.");
        return RedirectToAction(nameof(Index));
    }

    /// <summary>JSON list for the picker modal used by the product / blog / CMS forms.</summary>
    [HttpGet("/admin/thu-vien-anh/danh-sach")]
    public async Task<IActionResult> List()
    {
        var assets = await Db.MediaAssets
            .OrderByDescending(m => m.CreatedAt)
            .Take(200)
            .Select(m => new { id = m.Id, url = m.Url, name = m.FileName })
            .ToListAsync();
        return Ok(assets);
    }
}
