using System.Security.Claims;
using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Models.Checkout;
using Hoaii.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Hoaii.Web.Controllers;

public class CheckoutController(CartService cart, HoaiiDbContext db, SiteSettingsService settings, VnpayService vnpay, IServiceScopeFactory scopeFactory) : Controller
{
    private CheckoutViewModel BuildViewModel(CheckoutFormModel form, Models.Cart.CartViewModel cartModel) => new()
    {
        Form = form,
        Cart = cartModel,
        InnerCityFee = settings.GetDecimal(SiteSettingKeys.ShippingInnerCity),
        IntercityFee = settings.GetDecimal(SiteSettingKeys.ShippingIntercity),
        FreeShipThreshold = settings.GetDecimal(SiteSettingKeys.FreeShipThreshold),
        CodEnabled = settings.GetBool(SiteSettingKeys.PayCodEnabled),
        BankEnabled = settings.GetBool(SiteSettingKeys.PayBankEnabled),
        // The admin flag alone isn't enough — showing the option with no TmnCode/HashSecret set
        // would build a payment URL VNPAY rejects outright.
        VnpayEnabled = settings.GetBool(SiteSettingKeys.PayVnpayEnabled) && vnpay.IsConfigured,
        BankName = settings.Get(SiteSettingKeys.BankName),
        BankAccountNumber = settings.Get(SiteSettingKeys.BankAccountNumber),
        BankAccountHolder = settings.Get(SiteSettingKeys.BankAccountHolder),
        BankTransferNote = settings.Get(SiteSettingKeys.BankTransferNote),
    };

    public async Task<IActionResult> Index()
    {
        var cartModel = await cart.GetCartAsync();
        if (cartModel.Items.Count == 0)
        {
            return RedirectToAction("Index", "Cart");
        }

        return View(BuildViewModel(new CheckoutFormModel(), cartModel));
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
            return View("Index", BuildViewModel(form, cartModel));
        }

        var shippingMethod = form.ShippingMethod == "Intercity" ? ShippingMethod.Intercity : ShippingMethod.InnerCity;

        // Never accept a payment method the shop has switched off — fall back to whatever is on.
        var codEnabled = settings.GetBool(SiteSettingKeys.PayCodEnabled);
        var bankEnabled = settings.GetBool(SiteSettingKeys.PayBankEnabled);
        var vnpayEnabled = settings.GetBool(SiteSettingKeys.PayVnpayEnabled) && vnpay.IsConfigured;
        PaymentMethod paymentMethod = form.PaymentMethod switch
        {
            "CashOnDelivery" when codEnabled => PaymentMethod.CashOnDelivery,
            "Vnpay" when vnpayEnabled => PaymentMethod.Vnpay,
            "BankTransfer" when bankEnabled => PaymentMethod.BankTransfer,
            _ => bankEnabled ? PaymentMethod.BankTransfer : codEnabled ? PaymentMethod.CashOnDelivery : PaymentMethod.Vnpay,
        };

        // Recompute the shipping fee server-side from admin config — never trust a posted amount.
        var shippingFee = ShippingCalculator.Fee(
            form.ShippingMethod, cartModel.Subtotal,
            settings.GetDecimal(SiteSettingKeys.ShippingInnerCity),
            settings.GetDecimal(SiteSettingKeys.ShippingIntercity),
            settings.GetDecimal(SiteSettingKeys.FreeShipThreshold),
            cartModel.FreeShipping);

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
            ShippingFee = shippingFee,
            // Persist the discount and the code that produced it, so the total can be explained
            // after the fact — the voucher used to disappear the moment the order was placed.
            Discount = cartModel.Discount,
            VoucherCode = cartModel.AppliedVoucherCode,
            Vat = cartModel.Vat,
            Total = cartModel.Total + shippingFee,
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

        // Fire-and-forget: a live SMTP send takes 1-3s per email, and awaiting two-to-three of
        // them in a row (customer confirmation + one per internal recipient) before redirecting
        // was adding several real seconds to every checkout — most noticeably for VNPAY, where
        // that delay sits in front of a full-page navigation to another domain instead of a
        // same-site redirect. Runs in its own DI scope since the request's services (this
        // controller's `db`, `email`, `settings`) may already be disposed by the time it runs;
        // `order` itself is safe to read from — Items is a plain in-memory list, not a lazy nav.
        var adminUrl = $"{Request.Scheme}://{Request.Host}/admin/don-hang/{order.Id}";
        _ = Task.Run(async () =>
        {
            using var scope = scopeFactory.CreateScope();
            var bgEmail = scope.ServiceProvider.GetRequiredService<EmailSender>();
            var bgSettings = scope.ServiceProvider.GetRequiredService<SiteSettingsService>();

            var sends = new List<Task>();

            var (customerSubject, customerHtml) = OrderEmailTemplates.CustomerConfirmation(order);
            sends.Add(bgEmail.SendAsync(order.Email, customerSubject, customerHtml));

            // Nội bộ HOÀI: team Sales không đăng nhập /admin nên đây là kênh duy nhất họ biết có
            // đơn mới — cần đủ thông tin để liên hệ khách ngay, không phải mở admin ra mới xem được.
            var notifyEmails = bgSettings.Get(SiteSettingKeys.OrderNotifyEmails)
                .Split([',', ';', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (notifyEmails.Count > 0)
            {
                var (internalSubject, internalHtml) = OrderEmailTemplates.InternalNotification(order, adminUrl);
                sends.AddRange(notifyEmails.Select(to => bgEmail.SendAsync(to, internalSubject, internalHtml)));
            }

            try
            {
                await Task.WhenAll(sends);
            }
            catch { /* email must never break order placement — SendAsync itself already catches */ }
        });

        // Never mark this order Paid here — only VnpayController, once it has verified a real
        // signed callback from VNPAY, is allowed to do that (see WF-037).
        if (paymentMethod == PaymentMethod.Vnpay)
        {
            var returnUrl = $"{Request.Scheme}://{Request.Host}/thanh-toan/vnpay/tra-ve";
            return Redirect(vnpay.BuildPaymentUrl(order, ClientIpForVnpay(), returnUrl));
        }

        return RedirectToAction(nameof(Confirmation), new { orderNumber = order.OrderNumber });
    }

    /// <summary>vnp_IpAddr must be Alphanumeric[7,45] per VNPAY's spec — "::1" (IPv6 loopback,
    /// what Kestrel reports for local requests) is only 3 characters and gets the whole payment
    /// request silently rejected. Unwraps IPv4-mapped-IPv6 (::ffff:x.x.x.x, what some proxies
    /// send) and falls back to a real-looking placeholder for anything still too short.</summary>
    private string ClientIpForVnpay()
    {
        var addr = HttpContext.Connection.RemoteIpAddress;
        if (addr is not null && addr.IsIPv4MappedToIPv6)
        {
            addr = addr.MapToIPv4();
        }
        var s = addr?.ToString() ?? "";
        return s.Length >= 7 ? s : "127.0.0.1";
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

    public async Task<IActionResult> Confirmation(string orderNumber, string? vnpay)
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
            // Set only by VnpayController.Return — "failed" means a valid, signed VNPAY callback
            // reported the payment as unsuccessful (declined, cancelled, timed out, etc).
            VnpayPaymentFailed = vnpay == "failed",
        });
    }
}
