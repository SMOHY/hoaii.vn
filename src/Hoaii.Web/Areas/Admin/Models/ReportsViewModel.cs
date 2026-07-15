using Hoaii.Domain.Entities;

namespace Hoaii.Web.Areas.Admin.Models;

public class ReportsViewModel
{
    public decimal RevenueThisMonth { get; set; }
    public int OrdersThisMonth { get; set; }
    public decimal RevenueAllTime { get; set; }
    public int OrdersAllTime { get; set; }
    public decimal PaidRevenue { get; set; }

    public IReadOnlyList<StatusRow> StatusBreakdown { get; set; } = [];
    public IReadOnlyList<MonthRow> MonthlyRevenue { get; set; } = [];
    public IReadOnlyList<ProductRow> TopProducts { get; set; } = [];
    public IReadOnlyList<CustomerRow> TopCustomers { get; set; } = [];

    public decimal AvgOrderValue => OrdersAllTime > 0 ? Math.Round(RevenueAllTime / OrdersAllTime, 0) : 0;

    public class StatusRow { public OrderStatus Status { get; set; } public int Count { get; set; } public decimal Total { get; set; } }
    public class MonthRow { public required string Label { get; set; } public decimal Revenue { get; set; } }
    public class ProductRow { public required string Name { get; set; } public int Quantity { get; set; } public decimal Revenue { get; set; } }
    public class CustomerRow { public required string Email { get; set; } public int Orders { get; set; } public decimal Spent { get; set; } }
}
