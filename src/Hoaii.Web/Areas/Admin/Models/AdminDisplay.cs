using Hoaii.Domain.Entities;

namespace Hoaii.Web.Areas.Admin.Models;

/// <summary>Vietnamese labels + badge colours for the enums the admin screens show.</summary>
public static class AdminDisplay
{
    public static string OrderStatusLabel(OrderStatus s) => s switch
    {
        OrderStatus.Pending => "Chờ xác nhận",
        OrderStatus.Confirmed => "Chờ lấy hàng",
        OrderStatus.Shipping => "Đang giao hàng",
        OrderStatus.Delivered => "Đã giao",
        OrderStatus.Returned => "Trả hàng",
        OrderStatus.Cancelled => "Đã hủy",
        _ => s.ToString(),
    };

    public static string OrderStatusBadge(OrderStatus s) => s switch
    {
        OrderStatus.Pending => "amber",
        OrderStatus.Confirmed => "blue",
        OrderStatus.Shipping => "purple",
        OrderStatus.Delivered => "green",
        OrderStatus.Returned => "grey",
        OrderStatus.Cancelled => "red",
        _ => "grey",
    };

    public static string PaymentStatusLabel(PaymentStatus s) => s switch
    {
        PaymentStatus.Unpaid => "Chưa thanh toán",
        PaymentStatus.Paid => "Đã thanh toán",
        PaymentStatus.Refunded => "Đã hoàn tiền",
        _ => s.ToString(),
    };

    public static string PaymentStatusBadge(PaymentStatus s) => s switch
    {
        PaymentStatus.Unpaid => "grey",
        PaymentStatus.Paid => "green",
        PaymentStatus.Refunded => "amber",
        _ => "grey",
    };

    public static string PaymentMethodLabel(PaymentMethod m) => m switch
    {
        PaymentMethod.BankTransfer => "Chuyển khoản ngân hàng",
        PaymentMethod.CashOnDelivery => "Thanh toán khi nhận hàng",
        _ => m.ToString(),
    };

    public static string ShippingMethodLabel(ShippingMethod m) => m switch
    {
        ShippingMethod.InnerCity => "Nội thành Hà Nội",
        ShippingMethod.Intercity => "Vận chuyển liên tỉnh",
        _ => m.ToString(),
    };

    public static string BadgeLabel(ProductBadge b) => b switch
    {
        ProductBadge.New => "Hàng mới",
        ProductBadge.Sale => "Giảm giá",
        ProductBadge.OutOfStock => "Hết hàng",
        _ => "—",
    };

    /// <summary>"1.234.000đ"</summary>
    public static string Money(decimal amount) => amount.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("vi-VN")) + "đ";
}
