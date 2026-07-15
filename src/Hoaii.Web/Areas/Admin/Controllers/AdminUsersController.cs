using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Areas.Admin.Controllers;

// Managing admins is Owner-only.
[Authorize(Policy = AdminAuth.PolicyOwner)]
public class AdminUsersController(HoaiiDbContext db, AdminAuthService auth) : BaseAdminController(db)
{
    [HttpGet("/admin/tai-khoan")]
    public async Task<IActionResult> Index()
    {
        var users = await Db.AdminUsers.OrderBy(a => a.CreatedAt).ToListAsync();
        return View(users);
    }

    [HttpGet("/admin/tai-khoan/them")]
    public IActionResult Create() => View("Edit", new AdminUser { Email = "", FullName = "", PasswordHash = "" });

    [HttpGet("/admin/tai-khoan/{id:int}/sua")]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await Db.AdminUsers.FindAsync(id);
        if (user is null) return NotFound();
        return View(user);
    }

    [HttpPost("/admin/tai-khoan/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(int id, string email, string fullName, AdminRole role, bool isActive, string? password)
    {
        email = (email ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(fullName))
        {
            Fail("Email và tên không được để trống.");
            return RedirectToAction(id == 0 ? nameof(Create) : nameof(Edit), id == 0 ? null : new { id });
        }
        if (await Db.AdminUsers.AnyAsync(a => a.Email == email && a.Id != id))
        {
            Fail("Email đã được dùng cho tài khoản khác.");
            return RedirectToAction(id == 0 ? nameof(Create) : nameof(Edit), id == 0 ? null : new { id });
        }

        if (id == 0)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                Fail("Tài khoản mới cần đặt mật khẩu.");
                return RedirectToAction(nameof(Create));
            }
            var user = new AdminUser { Email = email, FullName = fullName.Trim(), Role = role, IsActive = isActive, CreatedAt = DateTime.UtcNow, PasswordHash = "" };
            user.PasswordHash = auth.HashPassword(user, password);
            Db.AdminUsers.Add(user);
            auth.Audit("Thêm admin", nameof(AdminUser), null, email);
            await Db.SaveChangesAsync();
            Ok("Đã tạo tài khoản admin.");
        }
        else
        {
            var user = await Db.AdminUsers.FindAsync(id);
            if (user is null) return NotFound();

            // An Owner can't strip their own Owner role or disable themselves and lock the shop out.
            var selfId = auth.CurrentAdminId();
            if (user.Id == selfId && (role != AdminRole.Owner || !isActive))
            {
                Fail("Không thể tự hạ quyền hoặc vô hiệu hoá tài khoản đang đăng nhập.");
                return RedirectToAction(nameof(Edit), new { id });
            }

            user.Email = email;
            user.FullName = fullName.Trim();
            user.Role = role;
            user.IsActive = isActive;
            if (!string.IsNullOrWhiteSpace(password))
            {
                user.PasswordHash = auth.HashPassword(user, password);
                user.FailedAttempts = 0;
                user.LockedUntil = null;
            }
            auth.Audit("Sửa admin", nameof(AdminUser), id, email);
            await Db.SaveChangesAsync();
            Ok("Đã lưu tài khoản.");
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/tai-khoan/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (id == auth.CurrentAdminId())
        {
            Fail("Không thể xóa tài khoản đang đăng nhập.");
            return RedirectToAction(nameof(Index));
        }
        var user = await Db.AdminUsers.FindAsync(id);
        if (user is null) return NotFound();

        // Never leave the shop with zero Owners.
        if (user.Role == AdminRole.Owner && await Db.AdminUsers.CountAsync(a => a.Role == AdminRole.Owner) <= 1)
        {
            Fail("Đây là chủ shop duy nhất, không thể xóa.");
            return RedirectToAction(nameof(Index));
        }

        Db.AdminUsers.Remove(user);
        auth.Audit("Xóa admin", nameof(AdminUser), id, user.Email);
        await Db.SaveChangesAsync();
        Ok("Đã xóa tài khoản.");
        return RedirectToAction(nameof(Index));
    }
}
