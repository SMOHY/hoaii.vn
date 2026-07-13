using System.ComponentModel.DataAnnotations;

namespace Hoaii.Web.Models.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = "";
}

public class VerifyOtpViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập mã xác thực")]
    public string Code { get; set; } = "";

    public string? ReturnUrl { get; set; }
    public string? Error { get; set; }
}

public class ProfileFieldRowViewModel
{
    public required string Label { get; init; }
    public required string DisplayValue { get; init; }
    public bool IsEditable { get; init; } = true;
    public required string EditUrl { get; init; } // e.g. "#edit-fullname-modal"
}

public class ProfileViewModel
{
    public required string FullNameOrPlaceholder { get; init; }
    public required string GenderDisplay { get; init; }
    public required string DateOfBirthDisplay { get; init; }
    public required string Email { get; init; }

    public string? FullNameValue { get; init; }
    public string? GenderValue { get; init; } // "Male" | "Female"
    public string? DateOfBirthValue { get; init; } // dd/MM/yyyy
}

public class OrderLineViewModel
{
    public string? ThumbnailUrl { get; init; }
    public required string Name { get; init; }
    public string? VariantLabel { get; init; }
    public required decimal Price { get; init; }
    public required int Quantity { get; init; }
}

public class OrderCardViewModel
{
    public required string OrderNumber { get; init; }
    public required DateTime OrderDate { get; init; }
    public required string StatusLabel { get; init; }
    public required string StatusVariant { get; init; } // css modifier: pending | confirmed | shipping | delivered | returned | cancelled
    public required decimal Total { get; init; }
    public required IReadOnlyList<OrderLineViewModel> Items { get; init; }
    public bool CanReorder { get; init; }
}

public class OrderStatusTabViewModel
{
    public required string Key { get; init; }
    public required string Label { get; init; }
}

public class OrderHistoryViewModel
{
    public required IReadOnlyList<OrderStatusTabViewModel> Tabs { get; init; }
    public required string ActiveStatus { get; init; }
    public required IReadOnlyList<OrderCardViewModel> Orders { get; init; }
    public string? SearchTerm { get; init; }
}

public class ProvinceOptionViewModel
{
    public required int Id { get; init; }
    public required string Name { get; init; }
}

public class WardOptionViewModel
{
    public required int Id { get; init; }
    public required int ProvinceId { get; init; }
    public required string Name { get; init; }
}

public class SavedAddressViewModel
{
    public required int Id { get; init; }
    public required string FullName { get; init; }
    public required string Phone { get; init; }
    public required string ProvinceName { get; init; }
    public required string WardName { get; init; }
    public required string AddressDetail { get; init; }
    public string? PostalCode { get; init; }
    public bool IsDefault { get; init; }
}

public class AddressFormModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string Phone { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng chọn tỉnh/thành phố")]
    public int? ProvinceId { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn phường/xã")]
    public int? WardId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ cụ thể")]
    public string AddressDetail { get; set; } = "";

    public string? PostalCode { get; set; }
}

public class AddressesPageViewModel
{
    public required IReadOnlyList<SavedAddressViewModel> Addresses { get; init; }
    public required AddressFormModel NewAddress { get; init; }
    public required IReadOnlyList<ProvinceOptionViewModel> Provinces { get; init; }
    public required IReadOnlyList<WardOptionViewModel> AllWards { get; init; }
}
