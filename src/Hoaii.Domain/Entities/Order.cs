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

public enum PaymentStatus
{
    Unpaid,
    Paid,
    Refunded,
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

    // The voucher used to vanish the moment the order was placed: the discount was subtracted
    // into Total and the code was never written down, so a disputed total could not be explained.
    public decimal Discount { get; set; }
    public string? VoucherCode { get; set; }

    public decimal Total { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

    /// <summary>Carrier tracking number, filled in by an admin when the order ships.</summary>
    public string? TrackingNumber { get; set; }

    /// <summary>Internal note. Never shown to the customer.</summary>
    public string? AdminNote { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<OrderItem> Items { get; set; } = [];
    public ICollection<OrderStatusHistory> StatusHistory { get; set; } = [];
}
