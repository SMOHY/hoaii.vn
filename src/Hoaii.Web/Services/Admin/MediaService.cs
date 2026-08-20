using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Hoaii.Web.Services.Admin;

/// <summary>
/// Handles image uploads for the admin area — the site has never had upload of any kind.
/// Files are validated by content, re-encoded, and recorded in the shared MediaAsset library.
/// </summary>
public class MediaService(HoaiiDbContext db, AdminAuthService auth, IWebHostEnvironment env)
{
    private const int MaxDimension = 1920;
    public const long MaxBytes = 5 * 1024 * 1024;
    public const string MaxSizeLabel = "5MB";

    /// <summary>Resolved in one shared place so uploads are always written to the same folder
    /// the /uploads route serves from — see SiteWebRoot.</summary>
    private string WebRoot => SiteWebRoot.For(env);

    public sealed record UploadResult(bool Ok, MediaAsset? Asset, string? Error);

    /// <summary>Sniffs the leading bytes rather than trusting the extension or Content-Type.</summary>
    private static string? DetectFormat(ReadOnlySpan<byte> b)
    {
        if (b.Length >= 3 && b[0] == 0xFF && b[1] == 0xD8 && b[2] == 0xFF) return "jpeg";
        if (b.Length >= 8 && b[0] == 0x89 && b[1] == 0x50 && b[2] == 0x4E && b[3] == 0x47) return "png";
        if (b.Length >= 12 && b[0] == 0x52 && b[1] == 0x49 && b[2] == 0x46 && b[3] == 0x46
            && b[8] == 0x57 && b[9] == 0x45 && b[10] == 0x42 && b[11] == 0x50) return "webp";
        // SVG is text; look for a root <svg somewhere near the top.
        var head = System.Text.Encoding.ASCII.GetString(b[..Math.Min(b.Length, 512)]).TrimStart();
        if (head.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase) || head.StartsWith("<svg", StringComparison.OrdinalIgnoreCase))
        {
            return head.Contains("<svg", StringComparison.OrdinalIgnoreCase) ? "svg" : null;
        }
        return null;
    }

    public async Task<UploadResult> UploadAsync(IFormFile file)
    {
        if (file.Length == 0)
        {
            return new UploadResult(false, null, "File rỗng.");
        }
        if (file.Length > MaxBytes)
        {
            return new UploadResult(false, null, $"File vượt quá {MaxSizeLabel}.");
        }

        // Read the whole file into memory once (capped at 10MB) and work from the byte array.
        // Rewinding IFormFile.OpenReadStream between the magic-byte sniff and the decode is
        // fragile — some stream implementations don't seek cleanly, which made even a valid
        // PNG fail to decode.
        byte[] bytes;
        await using (var input = file.OpenReadStream())
        await using (var ms = new MemoryStream())
        {
            await input.CopyToAsync(ms);
            bytes = ms.ToArray();
        }

        var format = DetectFormat(bytes);
        if (format is null)
        {
            return new UploadResult(false, null, "Chỉ nhận ảnh JPG, PNG, WEBP hoặc SVG.");
        }

        var now = DateTime.UtcNow;
        var relDir = $"/uploads/{now:yyyy}/{now:MM}";
        var absDir = Path.Combine(WebRoot, "uploads", now.ToString("yyyy"), now.ToString("MM"));
        Directory.CreateDirectory(absDir);

        string url;
        int width = 0, height = 0;
        long sizeBytes;

        if (format == "svg")
        {
            // Vector — store as-is (already re-validated as SVG above). No raster processing.
            var name = $"{Guid.NewGuid():N}.svg";
            var abs = Path.Combine(absDir, name);
            await File.WriteAllBytesAsync(abs, bytes);
            url = $"{relDir}/{name}";
            sizeBytes = bytes.Length;
        }
        else
        {
            // Raster — decode, cap the long edge, re-encode to webp. This both shrinks the file
            // and strips anything hostile hiding in a malformed original, since ImageSharp only
            // ever writes back a clean image it decoded itself. A file with the right magic
            // bytes but corrupt data (or a decode bomb) is rejected here rather than 500-ing.
            Image image;
            try
            {
                image = Image.Load(bytes);
            }
            catch (Exception)
            {
                return new UploadResult(false, null, "Ảnh không hợp lệ hoặc bị hỏng.");
            }

            using (image)
            {
                if (image.Width > MaxDimension || image.Height > MaxDimension)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(MaxDimension, MaxDimension),
                    }));
                }
                width = image.Width;
                height = image.Height;

                var name = $"{Guid.NewGuid():N}.webp";
                var abs = Path.Combine(absDir, name);
                await image.SaveAsWebpAsync(abs, new WebpEncoder { Quality = 82 });
                url = $"{relDir}/{name}";
                sizeBytes = new FileInfo(abs).Length;
            }
        }

        var asset = new MediaAsset
        {
            Url = url,
            FileName = Path.GetFileName(file.FileName),
            Width = width,
            Height = height,
            SizeBytes = sizeBytes,
            UploadedByAdminUserId = auth.CurrentAdminId(),
            CreatedAt = now,
        };
        db.MediaAssets.Add(asset);
        auth.Audit("Tải ảnh", nameof(MediaAsset), null, asset.FileName);
        await db.SaveChangesAsync();

        return new UploadResult(true, asset, null);
    }

    /// <summary>Deletes the DB row and the file on disk.</summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var asset = await db.MediaAssets.FindAsync(id);
        if (asset is null)
        {
            return false;
        }

        var abs = Path.Combine(WebRoot, asset.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(abs))
        {
            File.Delete(abs);
        }

        db.MediaAssets.Remove(asset);
        auth.Audit("Xóa ảnh", nameof(MediaAsset), id, asset.FileName);
        await db.SaveChangesAsync();
        return true;
    }

    // ---------- Tệp không phải ảnh: video giới thiệu và tài liệu PDF ----------
    //
    // Không đưa vào MediaAssets: thư viện ảnh là nơi chọn ảnh để chèn, trộn video và PDF vào đó
    // chỉ làm bộ chọn ảnh rối thêm. Hai hàm dưới chỉ ghi tệp và trả về đường dẫn; ai gọi thì tự
    // quyết lưu đường dẫn đó ở đâu.

    public const long MaxVideoBytes = 60 * 1024 * 1024;
    public const string MaxVideoLabel = "60MB";
    public const long MaxDocBytes = 20 * 1024 * 1024;
    public const string MaxDocLabel = "20MB";

    public sealed record FileResult(bool Ok, string? Url, string? FileName, long SizeBytes, string? Error);

    /// <summary>Video MP4/WebM. Kiểm bằng byte đầu tệp chứ không tin phần mở rộng.</summary>
    public async Task<FileResult> UploadVideoAsync(IFormFile file) =>
        await SaveRawAsync(file, "video", MaxVideoBytes, MaxVideoLabel, bytes =>
        {
            // MP4/MOV: "ftyp" ở byte thứ 4. WebM: chữ ký EBML.
            if (bytes.Length >= 12 && bytes[4] == 0x66 && bytes[5] == 0x74 && bytes[6] == 0x79 && bytes[7] == 0x70) return ".mp4";
            if (bytes.Length >= 4 && bytes[0] == 0x1A && bytes[1] == 0x45 && bytes[2] == 0xDF && bytes[3] == 0xA3) return ".webm";
            return null;
        }, "Chỉ nhận video MP4 hoặc WebM.");

    /// <summary>Tài liệu PDF.</summary>
    public async Task<FileResult> UploadDocumentAsync(IFormFile file) =>
        await SaveRawAsync(file, "tai-lieu", MaxDocBytes, MaxDocLabel, bytes =>
            bytes.Length >= 5 && bytes[0] == 0x25 && bytes[1] == 0x50 && bytes[2] == 0x44 && bytes[3] == 0x46
                ? ".pdf"
                : null,
            "Chỉ nhận tệp PDF.");

    private async Task<FileResult> SaveRawAsync(
        IFormFile file, string folder, long maxBytes, string maxLabel,
        Func<byte[], string?> sniff, string wrongTypeMessage)
    {
        if (file is null || file.Length == 0)
        {
            return new FileResult(false, null, null, 0, "Chưa chọn tệp.");
        }
        if (file.Length > maxBytes)
        {
            return new FileResult(false, null, null, 0, $"Tệp vượt quá {maxLabel}.");
        }

        // Chỉ cần vài byte đầu để nhận dạng; phần còn lại chép thẳng ra đĩa nên video 60MB
        // không phải nằm hết trong bộ nhớ.
        var head = new byte[16];
        await using (var peek = file.OpenReadStream())
        {
            _ = await peek.ReadAsync(head);
        }

        var ext = sniff(head);
        if (ext is null)
        {
            return new FileResult(false, null, null, 0, wrongTypeMessage);
        }

        var now = DateTime.UtcNow;
        var relDir = $"/uploads/{folder}/{now:yyyy}";
        var absDir = Path.Combine(SiteWebRoot.For(env), "uploads", folder, now.ToString("yyyy"));
        Directory.CreateDirectory(absDir);

        var name = $"{Guid.NewGuid():N}{ext}";
        var abs = Path.Combine(absDir, name);
        await using (var input = file.OpenReadStream())
        await using (var output = File.Create(abs))
        {
            await input.CopyToAsync(output);
        }

        var size = new FileInfo(abs).Length;
        return new FileResult(true, $"{relDir}/{name}", Path.GetFileName(file.FileName), size, null);
    }

    /// <summary>Xoá một tệp đã tải lên theo đường dẫn công khai của nó. Không có tệp thì thôi,
    /// không báo lỗi — bản ghi trong DB mới là thứ người dùng nhìn thấy.</summary>
    public void DeleteFile(string? publicUrl)
    {
        if (string.IsNullOrWhiteSpace(publicUrl) || !publicUrl.StartsWith("/uploads/", StringComparison.Ordinal))
        {
            return;
        }
        var abs = Path.Combine(SiteWebRoot.For(env), publicUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(abs))
        {
            File.Delete(abs);
        }
    }
}
