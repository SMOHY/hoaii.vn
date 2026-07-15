using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Areas.Admin.Controllers;

/// <summary>
/// Base for every admin screen except the login page: locks the whole area behind the admin
/// cookie + AdminOnly policy, and feeds the sidebar's pending-order badge on every page.
/// </summary>
[Area("Admin")]
[Authorize(AuthenticationSchemes = AdminAuth.Scheme, Policy = AdminAuth.PolicyAdmin)]
public abstract class BaseAdminController(HoaiiDbContext db) : Controller
{
    protected readonly HoaiiDbContext Db = db;

    protected void Ok(string message) => TempData["AdminOk"] = message;
    protected void Fail(string message) => TempData["AdminError"] = message;

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        ViewBag.PendingOrderCount = await Db.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
        await next();
    }
}
