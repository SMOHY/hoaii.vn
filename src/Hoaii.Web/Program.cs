using Hoaii.Infrastructure;
using Hoaii.Web.Services;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<HoaiiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7);
    options.Cookie.IsEssential = true;
});
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OtpService>();
builder.Services.AddScoped<AdminAuthService>();
builder.Services.AddScoped<MediaService>();
builder.Services.AddScoped<OrderWorkflowService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/tai-khoan/dang-nhap";
        options.AccessDeniedPath = "/tai-khoan/dang-nhap";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
        options.Cookie.Name = "hoaii.auth";
    })
    // A separate cookie for the admin area — its own name, its own login path, a shorter life.
    // Signing out as a customer never logs the admin out and vice-versa.
    .AddCookie(AdminAuth.Scheme, options =>
    {
        options.LoginPath = "/admin/dang-nhap";
        options.AccessDeniedPath = "/admin/khong-co-quyen";
        options.LogoutPath = "/admin/dang-xuat";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = "hoaii.admin";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AdminAuth.PolicyAdmin, policy => policy
        .AddAuthenticationSchemes(AdminAuth.Scheme)
        .RequireAuthenticatedUser());

    options.AddPolicy(AdminAuth.PolicyOwner, policy => policy
        .AddAuthenticationSchemes(AdminAuth.Scheme)
        .RequireAuthenticatedUser()
        .RequireClaim(AdminAuth.RoleClaim, nameof(Hoaii.Domain.Entities.AdminRole.Owner)));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "category",
    pattern: "danh-muc/{slug}",
    defaults: new { controller = "Category", action = "Index" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "product",
    pattern: "san-pham/{slug}",
    defaults: new { controller = "Product", action = "Details" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "cart-add",
    pattern: "gio-hang/them",
    defaults: new { controller = "Cart", action = "Add" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "cart-update",
    pattern: "gio-hang/cap-nhat",
    defaults: new { controller = "Cart", action = "UpdateQty" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "cart-remove",
    pattern: "gio-hang/xoa",
    defaults: new { controller = "Cart", action = "Remove" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "cart-apply-voucher",
    pattern: "gio-hang/ap-dung-ma",
    defaults: new { controller = "Cart", action = "ApplyVoucher" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "cart-remove-voucher",
    pattern: "gio-hang/xoa-ma",
    defaults: new { controller = "Cart", action = "RemoveVoucher" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "cart",
    pattern: "gio-hang",
    defaults: new { controller = "Cart", action = "Index" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "search",
    pattern: "tim-kiem",
    defaults: new { controller = "Search", action = "Index" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "blog-details",
    pattern: "blog/{slug}",
    defaults: new { controller = "Blog", action = "Details" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "blog",
    pattern: "blog",
    defaults: new { controller = "Blog", action = "Index" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "about-us",
    pattern: "ve-chung-toi",
    defaults: new { controller = "Page", action = "AboutUs" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "partners",
    pattern: "hop-tac",
    defaults: new { controller = "Page", action = "Partners" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "contact",
    pattern: "lien-he",
    defaults: new { controller = "Page", action = "Contact" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "policy",
    pattern: "chinh-sach/{slug}",
    defaults: new { controller = "Page", action = "Policy" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "checkout-place-order",
    pattern: "thanh-toan/dat-hang",
    defaults: new { controller = "Checkout", action = "PlaceOrder" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "checkout-confirmation",
    pattern: "thanh-toan/xac-nhan/{orderNumber}",
    defaults: new { controller = "Checkout", action = "Confirmation" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "checkout",
    pattern: "thanh-toan",
    defaults: new { controller = "Checkout", action = "Index" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "account-login",
    pattern: "tai-khoan/dang-nhap",
    defaults: new { controller = "Account", action = "Login" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "account-verify-otp",
    pattern: "tai-khoan/xac-thuc",
    defaults: new { controller = "Account", action = "VerifyOtp" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "account-resend-otp",
    pattern: "tai-khoan/gui-lai-ma",
    defaults: new { controller = "Account", action = "ResendOtp" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "account-logout",
    pattern: "tai-khoan/dang-xuat",
    defaults: new { controller = "Account", action = "Logout" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "account-orders",
    pattern: "tai-khoan/don-hang",
    defaults: new { controller = "Account", action = "Orders" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "account-addresses",
    pattern: "tai-khoan/dia-chi",
    defaults: new { controller = "Account", action = "Addresses" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "account",
    pattern: "tai-khoan",
    defaults: new { controller = "Account", action = "Profile" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// The admin area uses attribute routing (every route is spelled out on the controller), so it
// needs the attribute-route endpoint map in addition to the conventional routes above.
app.MapControllers();

// Create the first Owner account if none exists, so /admin is reachable on a fresh database.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HoaiiDbContext>();
    await AdminAuthService.EnsureSeedAdminAsync(db, app.Configuration);
}

app.Run();
