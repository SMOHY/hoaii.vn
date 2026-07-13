using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Models.Checkout;
using Hoaii.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Controllers;

public class CheckoutController(CartService cart, HoaiiDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var cartModel = await cart.GetCartAsync();
        if (cartModel.Items.Count == 0)
        {
            return RedirectToAction("Index", "Cart");
        }

        return View(new CheckoutViewModel
        {
            Form = new CheckoutFormModel(),
            Cart = cartModel,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(CheckoutFormModel form)
    {
        var cartModel = await cart.GetCartAsync();
        if (cartModel.Items.Count == 0)
        {
            return RedirectToAction("Index", "Cart");
        }

        if (!ModelState.IsValid)
        {
            return View("Index", new CheckoutViewModel { Form = form, Cart = cartModel });
        }

        var shippingMethod = form.ShippingMethod == "Intercity" ? ShippingMethod.Intercity : ShippingMethod.InnerCity;
        var paymentMethod = form.PaymentMethod == "CashOnDelivery" ? PaymentMethod.CashOnDelivery : PaymentMethod.BankTransfer;

        var order = new Order
        {
            OrderNumber = $"HD{DateTime.UtcNow:yyMMdd}{Random.Shared.Next(1000, 9999)}",
            Email = form.Email,
            FirstName = form.FirstName,
            LastName = form.LastName,
            CompanyName = form.CompanyName,
            Address = form.Address,
            ProvinceDistrictWard = form.ProvinceDistrictWard,
            Phone = form.Phone,
            Notes = form.Notes,
            ShippingMethod = shippingMethod,
            PaymentMethod = paymentMethod,
            Subtotal = cartModel.Subtotal,
            ShippingFee = 0,
            Total = cartModel.Total,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Items = cartModel.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductVariantId = i.VariantId,
                ProductName = i.Name,
                VariantName = i.VariantLabel,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
            }).ToList(),
        };

        db.Orders.Add(order);
        await db.SaveChangesAsync();

        cart.Clear();

        return RedirectToAction(nameof(Confirmation), new { orderNumber = order.OrderNumber });
    }

    public async Task<IActionResult> Confirmation(string orderNumber)
    {
        var order = await db.Orders.FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        if (order is null)
        {
            return NotFound();
        }

        return View(new OrderConfirmationViewModel
        {
            OrderNumber = order.OrderNumber,
            Total = order.Total,
            Email = order.Email,
        });
    }
}
