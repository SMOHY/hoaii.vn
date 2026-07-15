using Hoaii.Web.Areas.Admin.Models;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Hoaii.Web.Areas.Admin.Controllers;

/// <summary>Login / logout / access-denied. The only admin controller open to anonymous users.</summary>
[Area("Admin")]
public class AuthController(AdminAuthService auth) : Controller
{
    [HttpGet("/admin/dang-nhap")]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new AdminLoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost("/admin/dang-nhap")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AdminLoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (result, _) = await auth.SignInAsync(model.Email.Trim(), model.Password);
        model.Password = "";

        switch (result)
        {
            case AdminAuthService.SignInResult.Success:
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                return Redirect("/admin");

            case AdminAuthService.SignInResult.LockedOut:
                model.Error = "Tài khoản tạm khoá do nhập sai nhiều lần. Vui lòng thử lại sau 15 phút.";
                break;
            case AdminAuthService.SignInResult.Disabled:
                model.Error = "Tài khoản đã bị vô hiệu hoá.";
                break;
            default:
                model.Error = "Email hoặc mật khẩu không đúng.";
                break;
        }
        return View(model);
    }

    [HttpPost("/admin/dang-xuat")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await auth.SignOutAsync();
        return Redirect("/admin/dang-nhap");
    }

    [HttpGet("/admin/khong-co-quyen")]
    public IActionResult AccessDenied() => View();
}
