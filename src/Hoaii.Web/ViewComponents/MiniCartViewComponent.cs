using Hoaii.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hoaii.Web.ViewComponents;

public class MiniCartViewComponent(CartService cart) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = await cart.GetCartAsync();
        return View(model);
    }
}
