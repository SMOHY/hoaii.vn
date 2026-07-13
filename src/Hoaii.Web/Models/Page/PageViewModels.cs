using System.ComponentModel.DataAnnotations;

namespace Hoaii.Web.Models.Page;

public class ContactFormModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên")]
    public string FirstName { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập họ")]
    public string LastName { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = "";

    public string? Phone { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tin nhắn")]
    public string Message { get; set; } = "";
}

public class WholesaleFormModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên")]
    public string FirstName { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập họ")]
    public string LastName { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = "";

    public string? Phone { get; set; }
    public string? PostalCode { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên doanh nghiệp")]
    public string CompanyName { get; set; } = "";

    [Required]
    public string RequestType { get; set; } = "Business"; // "Business" | "CorporateGift"

    public string? Message { get; set; }
}

public class PolicyPageViewModel
{
    public required string Title { get; init; }
    public required string BreadcrumbLabel { get; init; }
}
