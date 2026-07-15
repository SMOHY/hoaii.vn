using System.Security.Claims;
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

        // Tie the order to the signed-in customer so it shows in their history even if they
        // later change the email on their account; guests still fall back to email matching.
        int? customerId = null;
        if (User.Identity?.IsAuthenticated == true
            && int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var cid))
        {
            customerId = cid;
        }

        var order = new Order
        {
            OrderNumber = await NextOrderNumberAsync(),
            CustomerId = customerId,
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
            // Persist the discount and the code that produced it, so the total can be explained
            // after the fact — the voucher used to disappear the moment the order was placed.
            Discount = cartModel.Discount,
            VoucherCode = cartModel.AppliedVoucherCode,
            Total = cartModel.Total,
            Status = OrderStatus.Pending,
            PaymentStatus = PaymentStatus.Unpaid,
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

        // Draw down stock for variants that track it (0 = untracked / made to order).
        foreach (var item in cartModel.Items.Where(i => i.VariantId is not null))
        {
            var variant = await db.ProductVariants.FirstOrDefaultAsync(v => v.Id == item.VariantId);
            if (variant is not null && variant.StockQuantity > 0)
            {
                variant.StockQuantity = Math.Max(0, variant.StockQuantity - item.Quantity);
            }
        }

        db.Orders.Add(order);

        // Count the redemption so per-code usage limits mean something.
        if (cartModel.AppliedVoucherCode is { } usedCode)
        {
            var voucher = await db.Vouchers.FirstOrDefaultAsync(v => v.Code == usedCode);
            if (voucher is not null)
            {
                voucher.UsedCount++;
            }
        }

        await db.SaveChangesAsync();

        cart.Clear();

        return RedirectToAction(nameof(Confirmation), new { orderNumber = order.OrderNumber });
    }

    /// <summary>
    /// Sequential daily number (HDyyMMdd-0001) rather than a random 4-digit suffix, which
    /// collided against the unique index roughly one order in a thousand per day.
    /// </summary>
    private async Task<string> NextOrderNumberAsync()
    {
        var prefix = $"HD{DateTime.UtcNow:yyMMdd}";
        var todayCount = await db.Orders.CountAsync(o => o.OrderNumber.StartsWith(prefix));
        return $"{prefix}-{todayCount + 1:D4}";
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
