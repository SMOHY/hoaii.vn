namespace Hoaii.Domain.Entities;

/// <summary>
/// An uploaded image. The site has never had upload of any kind — every image path in the
/// database was written by a migration — so this is also the shared picker the product,
/// blog and CMS screens all draw from.
/// </summary>
public class MediaAsset
{
    public int Id { get; set; }

    /// <summary>Public path, e.g. /uploads/2026/07/{guid}.webp.</summary>
    public required string Url { get; set; }

    /// <summary>Name the file had on the uploader's machine, so it can be found again by eye.</summary>
    public required string FileName { get; set; }

    public int Width { get; set; }
    public int Height { get; set; }
    public long SizeBytes { get; set; }

    public int? UploadedByAdminUserId { get; set; }
    public AdminUser? UploadedByAdminUser { get; set; }

    public DateTime CreatedAt { get; set; }
}
