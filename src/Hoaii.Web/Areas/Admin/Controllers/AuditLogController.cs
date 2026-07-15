using Hoaii.Infrastructure;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Areas.Admin.Controllers;

// Read-only, Owner-only.
[Authorize(Policy = AdminAuth.PolicyOwner)]
public class AuditLogController(HoaiiDbContext db) : BaseAdminController(db)
{
    private const int PageSize = 50;

    [HttpGet("/admin/nhat-ky")]
    public async Task<IActionResult> Index(int page = 1)
    {
        var total = await Db.AdminAuditLogs.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var logs = await Db.AdminAuditLogs
            .Include(l => l.AdminUser)
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        ViewBag.Page = page;
        ViewBag.TotalPages = totalPages;
        return View(logs);
    }
}
