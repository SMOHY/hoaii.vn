using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Areas.Admin.Controllers;

public class DashboardController(HoaiiDbContext db) : BaseAdminController(db)
{
    [HttpGet("/admin")]
    public async Task<IActionResult> Index()
    {
        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1);

        // Revenue counts paid, non-cancelled orders only.
        bool CountsAsRevenue(Order o) => o.Status != OrderStatus.Cancelled;

        var paidOrders = Db.Orders.Where(o => o.Status != OrderStatus.Cancelled);

        var model = new DashboardViewModel
        {
            RevenueToday = await paidOrders.Where(o => o.CreatedAt >= today).SumAsync(o => (decimal?)o.Total) ?? 0,
            RevenueMonth = await paidOrders.Where(o => o.CreatedAt >= monthStart).SumAsync(o => (decimal?)o.Total) ?? 0,
            OrdersToday = await Db.Orders.CountAsync(o => o.CreatedAt >= today),
            PendingCount = await Db.Orders.CountAsync(o => o.Status == OrderStatus.Pending),
            ProcessingCount = await Db.Orders.CountAsync(o => o.Status == OrderStatus.Confirmed || o.Status == OrderStatus.Shipping),
            CustomerCount = await Db.Customers.CountAsync(),
            ProductCount = await Db.Products.CountAsync(),
            RecentOrders = await Db.Orders
                .OrderByDescending(o => o.CreatedAt)
                .Take(8)
                .Select(o => new DashboardViewModel.OrderRow
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerName = o.LastName + " " + o.FirstName,
                    Total = o.Total,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                })
                .ToListAsync(),
            LowStock = await Db.ProductVariants
                .Where(v => v.StockQuantity <= 5)
                .OrderBy(v => v.StockQuantity)
                .Take(8)
                .Select(v => new DashboardViewModel.StockRow
                {
                    ProductId = v.ProductId,
                    ProductName = v.Product.Name,
                    VariantName = v.Name,
                    Stock = v.StockQuantity,
                })
                .ToListAsync(),
        };

        return View(model);
    }
}
