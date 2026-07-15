using System.Globalization;
using System.Security.Claims;
using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Models.Account;
using Hoaii.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Controllers;

public class AccountController(HoaiiDbContext db, OtpService otp) : Controller
{
    private static readonly (OrderStatus Status, string Label)[] StatusTabs =
    [
        (OrderStatus.Pending, "Chờ xác nhận"),
        (OrderStatus.Confirmed, "Chờ lấy hàng"),
        (OrderStatus.Shipping, "Đang giao hàng"),
        (OrderStatus.Delivered, "Đã giao"),
        (OrderStatus.Returned, "Trả hàng"),
        (OrderStatus.Cancelled, "Đã hủy"),
    ];

    // ---------- Login / OTP ----------

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(Profile));
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        await otp.SendAsync(model.Email);
        return RedirectToAction(nameof(VerifyOtp), new { email = model.Email, returnUrl });
    }

    [HttpGet]
    public IActionResult VerifyOtp(string email, string? returnUrl = null)
    {
        return View(new VerifyOtpViewModel { Email = email, ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model)
    {
        if (!otp.Verify(model.Email, model.Code))
        {
            model.Error = "Mã xác thực không đúng hoặc đã hết hạn.";
            return View(model);
        }

        var customer = await db.Customers.FirstOrDefaultAsync(c => c.Email == model.Email);
        if (customer is null)
        {
            customer = new Customer { Email = model.Email, CreatedAt = DateTime.UtcNow };
            db.Customers.Add(customer);
            await db.SaveChangesAsync();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new(ClaimTypes.Email, customer.Email),
            new(ClaimTypes.Name, customer.FullName ?? customer.Email),
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction(nameof(Profile));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendOtp(string email, string? returnUrl = null)
    {
        await otp.SendAsync(email);
        return RedirectToAction(nameof(VerifyOtp), new { email, returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }

    // ---------- Profile ----------

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var customer = await GetCurrentCustomerAsync();

        var model = new ProfileViewModel
        {
            FullNameOrPlaceholder = string.IsNullOrEmpty(customer.FullName) ? "Thêm thông tin" : customer.FullName,
            GenderDisplay = customer.Gender switch
            {
                Domain.Entities.Gender.Male => "Nam",
                Domain.Entities.Gender.Female => "Nữ",
                _ => "Thêm thông tin",
            },
            DateOfBirthDisplay = customer.DateOfBirth?.ToString("dd/MM/yyyy") ?? "Thêm thông tin",
            Email = customer.Email,
            FullNameValue = customer.FullName,
            GenderValue = customer.Gender?.ToString(),
            DateOfBirthValue = customer.DateOfBirth?.ToString("dd/MM/yyyy"),
        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateFullName(string value)
    {
        var customer = await GetCurrentCustomerAsync();
        customer.FullName = value.Trim();
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateGender(string value)
    {
        var customer = await GetCurrentCustomerAsync();
        if (Enum.TryParse<Domain.Entities.Gender>(value, out var gender))
        {
            customer.Gender = gender;
            await db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateDateOfBirth(string value)
    {
        var customer = await GetCurrentCustomerAsync();
        if (DateOnly.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            customer.DateOfBirth = date;
            await db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Profile));
    }

    // ---------- Order history ----------

    [Authorize]
    public async Task<IActionResult> Orders(string status = "Pending", string? q = null)
    {
        var email = User.FindFirstValue(ClaimTypes.Email)!;
        var customerId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var cid) ? cid : (int?)null;
        var parsedStatus = Enum.TryParse<OrderStatus>(status, out var s) ? s : OrderStatus.Pending;

        // Match by account id (orders placed while logged in) OR email (guest orders, and orders
        // from before this account existed).
        var query = db.Orders.Where(o =>
            (o.CustomerId == customerId || o.Email == email) && o.Status == parsedStatus);
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(o => o.OrderNumber.Contains(q) || o.Items.Any(i => i.ProductName.Contains(q)));
        }

        var orders = await query
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        var model = new OrderHistoryViewModel
        {
            Tabs = StatusTabs.Select(t => new OrderStatusTabViewModel { Key = t.Status.ToString(), Label = t.Label }).ToList(),
            ActiveStatus = parsedStatus.ToString(),
            SearchTerm = q,
            Orders = orders.Select(o => new OrderCardViewModel
            {
                OrderNumber = o.OrderNumber,
                OrderDate = o.CreatedAt,
                StatusLabel = StatusTabs.First(t => t.Status == o.Status).Label,
                StatusVariant = o.Status.ToString().ToLowerInvariant(),
                Total = o.Total,
                CanReorder = o.Status is OrderStatus.Delivered or OrderStatus.Cancelled or OrderStatus.Returned,
                Items = o.Items.Select(i => new OrderLineViewModel
                {
                    Name = i.ProductName,
                    VariantLabel = i.VariantName,
                    Price = i.UnitPrice,
                    Quantity = i.Quantity,
                }).ToList(),
            }).ToList(),
        };

        return View(model);
    }

    // ---------- Saved addresses ----------

    [Authorize]
    public async Task<IActionResult> Addresses()
    {
        var customer = await GetCurrentCustomerAsync();
        var model = await BuildAddressesPageViewModelAsync(customer.Id, new AddressFormModel());
        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAddress([Bind(Prefix = "NewAddress")] AddressFormModel form)
    {
        var customer = await GetCurrentCustomerAsync();

        if (form.ProvinceId is not null && form.WardId is not null)
        {
            var wardBelongsToProvince = await db.Wards.AnyAsync(w => w.Id == form.WardId && w.ProvinceId == form.ProvinceId);
            if (!wardBelongsToProvince)
            {
                ModelState.AddModelError(nameof(form.WardId), "Phường/xã không thuộc tỉnh/thành phố đã chọn");
            }
        }

        if (!ModelState.IsValid)
        {
            var model = await BuildAddressesPageViewModelAsync(customer.Id, form);
            return View(nameof(Addresses), model);
        }

        var isFirstAddress = !await db.Addresses.AnyAsync(a => a.CustomerId == customer.Id);
        db.Addresses.Add(new Address
        {
            CustomerId = customer.Id,
            FullName = form.FullName.Trim(),
            Phone = form.Phone.Trim(),
            ProvinceId = form.ProvinceId!.Value,
            WardId = form.WardId!.Value,
            AddressDetail = form.AddressDetail.Trim(),
            PostalCode = form.PostalCode,
            IsDefault = isFirstAddress,
        });
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Addresses));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveAddress(int id)
    {
        var customer = await GetCurrentCustomerAsync();
        var address = await db.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.CustomerId == customer.Id);
        if (address is not null)
        {
            db.Addresses.Remove(address);
            await db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Addresses));
    }

    // ---------- Helpers ----------

    private async Task<Customer> GetCurrentCustomerAsync()
    {
        var customerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await db.Customers.FirstAsync(c => c.Id == customerId);
    }

    private async Task<AddressesPageViewModel> BuildAddressesPageViewModelAsync(int customerId, AddressFormModel form)
    {
        var addresses = await db.Addresses
            .Where(a => a.CustomerId == customerId)
            .Include(a => a.Province)
            .Include(a => a.Ward)
            .OrderByDescending(a => a.IsDefault)
            .ToListAsync();

        var provinces = await db.Provinces.OrderBy(p => p.Name).ToListAsync();
        var wards = await db.Wards.OrderBy(w => w.Name).ToListAsync();

        return new AddressesPageViewModel
        {
            Addresses = addresses.Select(a => new SavedAddressViewModel
            {
                Id = a.Id,
                FullName = a.FullName,
                Phone = a.Phone,
                ProvinceName = a.Province.Name,
                WardName = a.Ward.Name,
                AddressDetail = a.AddressDetail,
                PostalCode = a.PostalCode,
                IsDefault = a.IsDefault,
            }).ToList(),
            NewAddress = form,
            Provinces = provinces.Select(p => new ProvinceOptionViewModel { Id = p.Id, Name = p.Name }).ToList(),
            AllWards = wards.Select(w => new WardOptionViewModel { Id = w.Id, ProvinceId = w.ProvinceId, Name = w.Name }).ToList(),
        };
    }
}
