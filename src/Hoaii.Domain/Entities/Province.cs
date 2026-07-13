namespace Hoaii.Domain.Entities;

/// <summary>
/// Vietnam's mid-2025 administrative reform collapsed the old 3-tier
/// (Tỉnh/Quận-huyện/Phường-xã) structure into 2 tiers (Tỉnh-Thành phố / Phường-xã).
/// This model targets the current 2-tier structure — seed data only covers a
/// representative sample of provinces/wards; a full official dataset import
/// is required before production use.
/// </summary>
public class Province
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public ICollection<Ward> Wards { get; set; } = [];
}
