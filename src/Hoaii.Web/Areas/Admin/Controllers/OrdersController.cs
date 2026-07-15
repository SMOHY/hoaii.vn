using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Areas.Admin.Models;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Areas.Admin.Controllers;

public class OrdersController(HoaiiDbContext db, OrderWorkflowService workflow, AdminAuthService auth)
    : BaseAdminController(db)
{
    private const int PageSize = 20;

    [HttpGet("/admin/don-hang")]
    public async Task<IActionResult> Index(OrderStatus? status, PaymentStatus? payment, string? q, int page = 1)
    {
        var query = Db.Orders.AsQueryable();

        if (status is not null)
        {
            query = query.Where(o => o.Status == status);
        }
        if (payment is not null)
        {
            query = query.Where(o => o.PaymentStatus == payment);
        }
        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(o =>
                o.OrderNumber.Contains(term) ||
                o.Email.Contains(term) ||
                o.Phone.Contains(term) ||
                (o.FirstName + " " + o.LastName).Contains(term));
        }

        var total = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var rows = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(o => new OrderListViewModel.Row
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerName = o.LastName + " " + o.FirstName,
                Phone = o.Phone,
                ItemCount = o.Items.Count,
                Total = o.Total,
                Status = o.Status,
                PaymentStatus = o.PaymentStatus,
                CreatedAt = o.CreatedAt,
            })
            .ToListAsync();

        return View(new OrderListViewModel
        {
            Orders = rows,
            Page = page,
            TotalPages = totalPages,
            TotalCount = total,
            Status = status,
            Payment = payment,
            Query = q,
        });
    }

    [HttpGet("/admin/don-hang/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var order = await Db.Orders
            .Include(o => o.Items)
            .Include(o => o.StatusHistory).ThenInclude(h => h.AdminUser)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
        {
            return NotFound();
        }

        return View(new OrderDetailViewModel
        {
            Order = order,
            NextStatuses = OrderWorkflowService.NextStatuses(order.Status),
        });
    }

    [HttpPost("/admin/don-hang/{id:int}/trang-thai")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(int id, OrderStatus to, string? note)
    {
        var order = await Db.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order is null)
        {
            return NotFound();
        }

        if (!workflow.ChangeStatus(order, to, note))
        {
            Fail("Không thể chuyển sang trạng thái đó từ trạng thái hiện tại.");
            return RedirectToAction(nameof(Details), new { id });
        }

        await Db.SaveChangesAsync();
        Ok($"Đã cập nhật trạng thái đơn {order.OrderNumber}.");
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost("/admin/don-hang/{id:int}/thanh-toan")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkPaid(int id, PaymentStatus paymentStatus)
    {
        var order = await Db.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order is null)
        {
            return NotFound();
        }

        order.PaymentStatus = paymentStatus;
        order.UpdatedAt = DateTime.UtcNow;
        auth.Audit("Cập nhật thanh toán", nameof(Order), order.Id, AdminDisplay.PaymentStatusLabel(paymentStatus));
        await Db.SaveChangesAsync();
        Ok("Đã cập nhật tình trạng thanh toán.");
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost("/admin/don-hang/{id:int}/cap-nhat")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateMeta(int id, string? trackingNumber, string? adminNote)
    {
        var order = await Db.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order is null)
        {
            return NotFound();
        }

        order.TrackingNumber = string.IsNullOrWhiteSpace(trackingNumber) ? null : trackingNumber.Trim();
        order.AdminNote = string.IsNullOrWhiteSpace(adminNote) ? null : adminNote.Trim();
        order.UpdatedAt = DateTime.UtcNow;
        auth.Audit("Cập nhật đơn", nameof(Order), order.Id, "Mã vận đơn / ghi chú");
        await Db.SaveChangesAsync();
        Ok("Đã lưu.");
        return RedirectToAction(nameof(Details), new { id });
    }
}
