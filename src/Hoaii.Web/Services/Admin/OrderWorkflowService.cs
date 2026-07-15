using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;

namespace Hoaii.Web.Services.Admin;

/// <summary>
/// The one place order status is allowed to change. Guards the transitions so an order can't
/// jump from Pending straight to Delivered, and records every move in OrderStatusHistory —
/// without which "when did this ship?" has no answer.
/// </summary>
public class OrderWorkflowService(HoaiiDbContext db, AdminAuthService auth)
{
    // Which statuses each status may move to. Anything not listed is rejected.
    private static readonly Dictionary<OrderStatus, OrderStatus[]> Allowed = new()
    {
        [OrderStatus.Pending] = [OrderStatus.Confirmed, OrderStatus.Cancelled],
        [OrderStatus.Confirmed] = [OrderStatus.Shipping, OrderStatus.Cancelled],
        [OrderStatus.Shipping] = [OrderStatus.Delivered],
        [OrderStatus.Delivered] = [OrderStatus.Returned],
        [OrderStatus.Returned] = [],
        [OrderStatus.Cancelled] = [],
    };

    public static IReadOnlyList<OrderStatus> NextStatuses(OrderStatus current) =>
        Allowed.TryGetValue(current, out var next) ? next : [];

    public static bool CanTransition(OrderStatus from, OrderStatus to) =>
        Allowed.TryGetValue(from, out var next) && next.Contains(to);

    /// <summary>
    /// Moves the order and writes history. Returns false (without touching the DB) if the move
    /// isn't allowed. Caller is responsible for SaveChanges.
    /// </summary>
    public bool ChangeStatus(Order order, OrderStatus to, string? note)
    {
        if (order.Status == to || !CanTransition(order.Status, to))
        {
            return false;
        }

        var from = order.Status;
        order.Status = to;
        order.UpdatedAt = DateTime.UtcNow;

        // Cash-on-delivery is settled the moment it's marked delivered.
        if (to == OrderStatus.Delivered && order.PaymentMethod == PaymentMethod.CashOnDelivery)
        {
            order.PaymentStatus = PaymentStatus.Paid;
        }

        db.OrderStatusHistories.Add(new OrderStatusHistory
        {
            OrderId = order.Id,
            FromStatus = from,
            ToStatus = to,
            AdminUserId = auth.CurrentAdminId(),
            Note = note,
            CreatedAt = DateTime.UtcNow,
        });

        auth.Audit("Đổi trạng thái đơn", nameof(Order), order.Id, $"{from} → {to}");
        return true;
    }
}
