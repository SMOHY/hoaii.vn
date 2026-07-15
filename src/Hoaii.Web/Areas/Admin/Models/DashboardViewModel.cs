using Hoaii.Domain.Entities;

namespace Hoaii.Web.Areas.Admin.Models;

public class DashboardViewModel
{
    public decimal RevenueToday { get; init; }
    public decimal RevenueMonth { get; init; }
    public int OrdersToday { get; init; }
    public int PendingCount { get; init; }
    public int ProcessingCount { get; init; }
    public int CustomerCount { get; init; }
    public int ProductCount { get; init; }

    public IReadOnlyList<OrderRow> RecentOrders { get; init; } = [];
    public IReadOnlyList<StockRow> LowStock { get; init; } = [];

    public class OrderRow
    {
        public int Id { get; init; }
        public required string OrderNumber { get; init; }
        public required string CustomerName { get; init; }
        public decimal Total { get; init; }
        public OrderStatus Status { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    public class StockRow
    {
        public int ProductId { get; init; }
        public required string ProductName { get; init; }
        public required string VariantName { get; init; }
        public int Stock { get; init; }
    }
}
