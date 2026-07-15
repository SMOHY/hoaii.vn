using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Areas.Admin.Controllers;

public class ReportsController(HoaiiDbContext db) : BaseAdminController(db)
{
    [HttpGet("/admin/bao-cao")]
    public async Task<IActionResult> Index()
    {
        // "Realized" revenue excludes cancelled and returned orders.
        var realized = Db.Orders.Where(o => o.Status != OrderStatus.Cancelled && o.Status != OrderStatus.Returned);

        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var vm = new ReportsViewModel
        {
            RevenueThisMonth = await realized.Where(o => o.CreatedAt >= monthStart).SumAsync(o => (decimal?)o.Total) ?? 0,
            OrdersThisMonth = await realized.CountAsync(o => o.CreatedAt >= monthStart),
            RevenueAllTime = await realized.SumAsync(o => (decimal?)o.Total) ?? 0,
            OrdersAllTime = await realized.CountAsync(),
            PaidRevenue = await Db.Orders.Where(o => o.PaymentStatus == PaymentStatus.Paid).SumAsync(o => (decimal?)o.Total) ?? 0,

            StatusBreakdown = await Db.Orders
                .GroupBy(o => o.Status)
                .Select(g => new ReportsViewModel.StatusRow { Status = g.Key, Count = g.Count(), Total = g.Sum(o => o.Total) })
                .ToListAsync(),

            TopProducts = await Db.OrderItems
                .Where(i => i.Order!.Status != OrderStatus.Cancelled && i.Order.Status != OrderStatus.Returned)
                .GroupBy(i => i.ProductName)
                .Select(g => new ReportsViewModel.ProductRow
                {
                    Name = g.Key,
                    Quantity = g.Sum(i => i.Quantity),
                    Revenue = g.Sum(i => i.UnitPrice * i.Quantity),
                })
                .OrderByDescending(r => r.Quantity)
                .Take(10)
                .ToListAsync(),

            TopCustomers = await realized
                .GroupBy(o => o.Email)
                .Select(g => new ReportsViewModel.CustomerRow
                {
                    Email = g.Key,
                    Orders = g.Count(),
                    Spent = g.Sum(o => o.Total),
                })
                .OrderByDescending(r => r.Spent)
                .Take(10)
                .ToListAsync(),
        };

        // Revenue for the last 6 months (oldest → newest), computed in memory to avoid provider
        // date-grouping quirks.
        var since = monthStart.AddMonths(-5);
        var recent = await realized.Where(o => o.CreatedAt >= since)
            .Select(o => new { o.CreatedAt, o.Total })
            .ToListAsync();
        vm.MonthlyRevenue = Enumerable.Range(0, 6)
            .Select(i => since.AddMonths(i))
            .Select(m => new ReportsViewModel.MonthRow
            {
                Label = m.ToString("MM/yyyy"),
                Revenue = recent.Where(o => o.CreatedAt.Year == m.Year && o.CreatedAt.Month == m.Month).Sum(o => o.Total),
            })
            .ToList();

        return View(vm);
    }
}
