using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
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
    private const long MaxBytes = 10 * 1024 * 1024;

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
            return new UploadResult(false, null, "File vượt quá 10MB.");
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
        var absDir = Path.Combine(env.WebRootPath, "uploads", now.ToString("yyyy"), now.ToString("MM"));
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

        var abs = Path.Combine(env.WebRootPath, asset.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(abs))
        {
            File.Delete(abs);
        }

        db.MediaAssets.Remove(asset);
        auth.Audit("Xóa ảnh", nameof(MediaAsset), id, asset.FileName);
        await db.SaveChangesAsync();
        return true;
    }
}
