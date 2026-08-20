using Hoaii.Infrastructure;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hoaii.Web.Areas.Admin.Filters;

namespace Hoaii.Web.Areas.Admin.Controllers;

public class MediaController(HoaiiDbContext db, MediaService media, IWebHostEnvironment env) : BaseAdminController(db)
{
    [HttpGet("/admin/thu-vien-anh")]
    public async Task<IActionResult> Index()
    {
        var assets = await Db.MediaAssets.OrderByDescending(m => m.CreatedAt).Take(200).ToListAsync();

        // A blank thumbnail looks the same whether the file vanished from disk or the /uploads
        // route is pointing at the wrong folder, and the two need very different fixes. Check the
        // disk here so the page can say which one it is instead of showing an empty grey box.
        var webRoot = Hoaii.Web.Services.SiteWebRoot.For(env);
        ViewBag.Missing = assets
            .Where(a => !System.IO.File.Exists(Path.Combine(webRoot, a.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar))))
            .Select(a => a.Id)
            .ToHashSet();

        return View(assets);
    }

    // Giới hạn tính cho CẢ lô: giao diện cho chọn nhiều ảnh một lần, mà trần 6MB cũ áp lên
    // toàn bộ request nên chọn hai ảnh 4MB là đứt, dù mỗi ảnh đều dưới mức 5MB được ghi trên
    // màn hình. Từng ảnh vẫn bị MediaService kiểm riêng đúng 5MB.
    [HttpPost("/admin/thu-vien-anh/tai-len")]
    [GioiHanTep(MediaService.MaxBytes * 10, "50MB cho một lần tải")]
    [ValidateAntiForgeryToken]
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
