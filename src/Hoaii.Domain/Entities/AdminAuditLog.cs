namespace Hoaii.Domain.Entities;

/// <summary>
/// Who changed what, and when. The site takes payments, so every write from the admin area is
/// recorded — otherwise a disputed price or a cancelled order has no paper trail.
/// </summary>
public class AdminAuditLog
{
    public int Id { get; set; }

    public int? AdminUserId { get; set; }
    public AdminUser? AdminUser { get; set; }

    /// <summary>Free text, e.g. "Cập nhật sản phẩm", "Đổi trạng thái đơn".</summary>
    public required string Action { get; set; }

    /// <summary>Entity name, e.g. "Product", "Order".</summary>
    public required string EntityType { get; set; }
    public int? EntityId { get; set; }

    /// <summary>Human-readable summary of the change, shown in the log screen.</summary>
    public string? Detail { get; set; }

    public DateTime CreatedAt { get; set; }
}
