using System.ComponentModel.DataAnnotations;
using Hoaii.Web.Models.Cart;

namespace Hoaii.Web.Models.Checkout;

public class CheckoutViewModel
{
    public required CheckoutFormModel Form { get; init; }
    public required CartViewModel Cart { get; init; }
    public decimal ShippingFee { get; init; } = 0; // "Miễn phí" per design-specs
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
