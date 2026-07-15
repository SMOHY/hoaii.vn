using Hoaii.Domain.Entities;

namespace Hoaii.Web.Areas.Admin.Models;

public class OrderListViewModel
{
    public IReadOnlyList<Row> Orders { get; init; } = [];
    public int Page { get; init; }
    public int TotalPages { get; init; }
    public int TotalCount { get; init; }

    // Current filter state, so the form and the pager links keep it.
    public OrderStatus? Status { get; init; }
    public PaymentStatus? Payment { get; init; }
    public string? Query { get; init; }

    public class Row
    {
        public int Id { get; init; }
        public required string OrderNumber { get; init; }
        public required string CustomerName { get; init; }
        public required string Phone { get; init; }
        public int ItemCount { get; init; }
        public decimal Total { get; init; }
        public OrderStatus Status { get; init; }
        public PaymentStatus PaymentStatus { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}

public class OrderDetailViewModel
{
    public required Order Order { get; init; }
    public IReadOnlyList<OrderStatus> NextStatuses { get; init; } = [];
}
