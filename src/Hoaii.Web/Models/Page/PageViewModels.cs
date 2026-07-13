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
    /// <summary>Page heading, e.g. "CHÍNH SÁCH ĐỔI TRẢ &amp; HOÀN TÁC".</summary>
    public required string Title { get; init; }

    /// <summary>Nav/footer label, which is shorter than the heading.</summary>
    public required string NavLabel { get; init; }

    public required string BreadcrumbLabel { get; init; }

    /// <summary>Body copy, in document order.</summary>
    public required IReadOnlyList<PolicyBlock> Blocks { get; init; }
}

public class PolicyBlock
{
    public required PolicyBlockKind Kind { get; init; }
    public required string Text { get; init; }
}

public enum PolicyBlockKind
{
    Paragraph,
    Heading,
    Bullet,
}
