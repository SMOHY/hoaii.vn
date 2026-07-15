using System.ComponentModel.DataAnnotations;

namespace Hoaii.Web.Areas.Admin.Models;

public class AdminLoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email chưa đúng định dạng.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    public string Password { get; set; } = "";

    public string? Error { get; set; }
    public string? ReturnUrl { get; set; }
}
