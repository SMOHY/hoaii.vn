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

    /// <summary>
    /// With JS on, cart.js posts these forms in the background and then re-fetches the page to
    /// swap the cart regions in place, so there is nothing to redirect to. Without JS the forms
    /// still post normally and land back where they came from.
    /// </summary>
    private IActionResult SafeRedirect(string? returnUrl)
    {
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
        {
            return NoContent();
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }
}
