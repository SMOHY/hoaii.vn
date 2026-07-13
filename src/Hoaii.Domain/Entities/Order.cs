namespace Hoaii.Domain.Entities;

public enum OrderStatus
{
    Pending,
    Confirmed,
    Shipping,
    Delivered,
    Returned,
    Cancelled,
}

public enum ShippingMethod
{
    InnerCity,
    Intercity,
}

public enum PaymentMethod
{
    BankTransfer,
    CashOnDelivery,
}

public class Order
{
    public int Id { get; set; }
    public required string OrderNumber { get; set; }

    // Set when the order was placed while logged in; historical guest orders
    // (and orders placed by guests who later create an account) are matched by Email instead.
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? CompanyName { get; set; }
    public required string Address { get; set; }
    public required string ProvinceDistrictWard { get; set; }
    public required string Phone { get; set; }
    public string? Notes { get; set; }

    public ShippingMethod ShippingMethod { get; set; }
    public PaymentMethod PaymentMethod { get; set; }

    public decimal Subtotal { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal Total { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; set; }

    public ICollection<OrderItem> Items { get; set; } = [];
}
