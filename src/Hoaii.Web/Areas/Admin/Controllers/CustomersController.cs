using Hoaii.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Areas.Admin.Controllers;

public class CustomersController(HoaiiDbContext db) : BaseAdminController(db)
{
    private const int PageSize = 30;

    [HttpGet("/admin/khach-hang")]
    public async Task<IActionResult> Index(string? q, int page = 1)
    {
        var query = Db.Customers.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(c => c.Email.Contains(term) || (c.FullName != null && c.FullName.Contains(term)));
        }

        var total = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        // Order count / spend is matched the same way the storefront does: by account id or email.
        var rows = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(c => new
            {
                c.Id, c.Email, c.FullName, c.CreatedAt,
                OrderCount = Db.Orders.Count(o => o.CustomerId == c.Id || o.Email == c.Email),
                Spend = Db.Orders.Where(o => (o.CustomerId == c.Id || o.Email == c.Email)
                    && o.Status != Hoaii.Domain.Entities.OrderStatus.Cancelled).Sum(o => (decimal?)o.Total) ?? 0,
            })
            .ToListAsync();

        ViewBag.Page = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.Total = total;
        ViewBag.Query = q;
        return View(rows.Select(r => (r.Id, r.Email, r.FullName, r.CreatedAt, r.OrderCount, r.Spend)).ToList());
    }

    [HttpGet("/admin/khach-hang/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var customer = await Db.Customers
            .Include(c => c.Addresses).ThenInclude(a => a.Province)
            .Include(c => c.Addresses).ThenInclude(a => a.Ward)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (customer is null) return NotFound();

        ViewBag.Orders = await Db.Orders
            .Where(o => o.CustomerId == id || o.Email == customer.Email)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return View(customer);
    }
}
