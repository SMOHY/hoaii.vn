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
    public async Task<IActionResult> Add(int productId, int? variantId, int quantity = 1, string? returnUrl = null)
    {
        if (!await cart.AddItemAsync(productId, variantId, quantity))
        {
            // Hết hàng / đã ẩn. Phải nói thẳng: im lặng bỏ qua thì khách bấm lại mãi vì tưởng nút
            // hỏng. Với JS bật, SafeRedirect trả 204 — mà 204 là res.ok, nên cart-live.js sẽ khoe
            // "Đã thêm vào giỏ hàng" cho một cú thêm vừa bị từ chối. 409 mới là sự thật.
            const string message = "Sản phẩm này hiện đã hết hàng nên chưa thể thêm vào giỏ.";
            if (IsAjax)
            {
                return Conflict(new { message });
            }

            TempData["CartError"] = message;
        }

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
    public async Task<IActionResult> ApplyVoucher(string code, string? returnUrl = null)
    {
        if (!string.IsNullOrWhiteSpace(code) && !await cart.ApplyVoucherAsync(code.Trim()))
        {
            TempData["VoucherError"] = "Mã không hợp lệ hoặc chưa đủ điều kiện áp dụng.";
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
    private bool IsAjax => Request.Headers.XRequestedWith == "XMLHttpRequest";

    private IActionResult SafeRedirect(string? returnUrl)
    {
        if (IsAjax)
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
