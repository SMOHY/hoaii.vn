namespace Hoaii.Domain.Entities;

/// <summary>
/// One row per status change. Without this an order's past is unknowable: the customer's order
/// tabs read <see cref="Order.Status"/> only, so "when did this ship?" would have no answer.
/// </summary>
public class OrderStatusHistory
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public OrderStatus FromStatus { get; set; }
    public OrderStatus ToStatus { get; set; }

    /// <summary>Null for changes the system made rather than a person.</summary>
    public int? AdminUserId { get; set; }
    public AdminUser? AdminUser { get; set; }

    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
}
