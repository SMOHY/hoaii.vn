using System.ComponentModel.DataAnnotations;
using Hoaii.Web.Models.Cart;

namespace Hoaii.Web.Models.Checkout;

public class CheckoutViewModel
{
    public required CheckoutFormModel Form { get; init; }
    public required CartViewModel Cart { get; init; }

    // Shipping config (admin-managed). The view shows the fee for the selected method and lets
    // JS switch between them; the server recomputes authoritatively on order placement.
    public decimal InnerCityFee { get; init; }
    public decimal IntercityFee { get; init; }
    public decimal FreeShipThreshold { get; init; }

    /// <summary>Fee for the currently selected method, after free-ship rules.</summary>
    public decimal ShippingFee => ShippingCalculator.Fee(
        Form.ShippingMethod, Cart.Subtotal, InnerCityFee, IntercityFee, FreeShipThreshold, Cart.FreeShipping);

    public bool ShipFree => ShippingFee == 0m;
    public decimal GrandTotal => Cart.Total + ShippingFee;

    // Payment config (admin-managed).
    public bool CodEnabled { get; init; } = true;
    public bool BankEnabled { get; init; } = true;
    public bool VnpayEnabled { get; init; }
    public string BankName { get; init; } = "";
    public string BankAccountNumber { get; init; } = "";
    public string BankAccountHolder { get; init; } = "";
    public string BankTransferNote { get; init; } = "";

    public bool HasBankInfo =>
        !string.IsNullOrWhiteSpace(BankName) || !string.IsNullOrWhiteSpace(BankAccountNumber);
}

public static class ShippingCalculator
{
    public static decimal Fee(string? method, decimal subtotal, decimal innerFee, decimal interFee,
        decimal freeThreshold, bool freeShipVoucher)
    {
        if (freeShipVoucher) return 0m;
        if (freeThreshold > 0 && subtotal >= freeThreshold) return 0m;
        return method == "Intercity" ? interFee : innerFee;
    }
}

public class CheckoutFormModel
{
    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập tên")]
    public string FirstName { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập họ")]
    public string LastName { get; set; } = "";

    public string? CompanyName { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
    public string Address { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập tỉnh/thành, quận/huyện, phường/xã")]
    public string ProvinceDistrictWard { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string Phone { get; set; } = "";

    public string? Notes { get; set; }

    [Required]
    public string ShippingMethod { get; set; } = "InnerCity"; // "InnerCity" | "Intercity"

    [Required]
    public string PaymentMethod { get; set; } = "BankTransfer"; // "BankTransfer" | "CashOnDelivery"
}

public class OrderConfirmationViewModel
{
    public required string OrderNumber { get; init; }
    public required decimal Total { get; init; }
    public required string Email { get; init; }
}
