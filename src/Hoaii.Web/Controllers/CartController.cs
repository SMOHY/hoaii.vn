using Hoaii.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hoaii.Web.Controllers;

public class CartController(CartService cart) : Controller
{
    public async Task<IActionResult> Index()
    {
        var model = await cart.GetCartAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add(int productId, int? variantId, int quantity = 1, string? returnUrl = null)
    {
        cart.AddItem(productId, variantId, quantity);
        return SafeRedirect(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateQty(int productId, int? variantId, int quantity, string? returnUrl = null)
    {
        cart.UpdateQuantity(productId, variantId, quantity);
        return SafeRedirect(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int productId, int? variantId, string? returnUrl = null)
    {
        cart.RemoveItem(productId, variantId);
        return SafeRedirect(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ApplyVoucher(string code, string? returnUrl = null)
    {
        if (!string.IsNullOrWhiteSpace(code))
        {
            cart.ApplyVoucher(code.Trim());
        }

        return SafeRedirect(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveVoucher(string? returnUrl = null)
    {
        cart.RemoveVoucher();
        return SafeRedirect(returnUrl);
    }

    private IActionResult SafeRedirect(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }
}
