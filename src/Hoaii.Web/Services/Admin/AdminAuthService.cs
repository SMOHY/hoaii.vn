using System.Security.Claims;
using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Services.Admin;

/// <summary>
/// Password sign-in for the admin area. Deliberately separate from the customer OTP flow.
/// </summary>
public class AdminAuthService(HoaiiDbContext db, IHttpContextAccessor http)
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    private static readonly PasswordHasher<AdminUser> Hasher = new();

    public string HashPassword(AdminUser user, string password) => Hasher.HashPassword(user, password);

    public enum SignInResult { Success, InvalidCredentials, LockedOut, Disabled }

    public async Task<(SignInResult Result, AdminUser? User)> SignInAsync(string email, string password)
    {
        var user = await db.AdminUsers.FirstOrDefaultAsync(a => a.Email == email);

        // Same generic answer whether the email is unknown or the password is wrong, so the
        // form can't be used to discover which admin emails exist.
        if (user is null)
        {
            return (SignInResult.InvalidCredentials, null);
        }

        if (!user.IsActive)
        {
            return (SignInResult.Disabled, null);
        }

        if (user.LockedUntil is { } until && until > DateTime.UtcNow)
        {
            return (SignInResult.LockedOut, user);
        }

        var verify = Hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verify == PasswordVerificationResult.Failed)
        {
            user.FailedAttempts++;
            if (user.FailedAttempts >= MaxFailedAttempts)
            {
                user.LockedUntil = DateTime.UtcNow.Add(LockoutDuration);
                user.FailedAttempts = 0;
            }
            await db.SaveChangesAsync();
            return (user.LockedUntil is not null ? SignInResult.LockedOut : SignInResult.InvalidCredentials, user);
        }

        // Success — reset the counters and rehash if the algorithm has since been strengthened.
        user.FailedAttempts = 0;
        user.LockedUntil = null;
        user.LastLoginAt = DateTime.UtcNow;
        if (verify == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = Hasher.HashPassword(user, password);
        }
        await db.SaveChangesAsync();

        await IssueCookieAsync(user);
        return (SignInResult.Success, user);
    }

    private async Task IssueCookieAsync(AdminUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
            new(AdminAuth.RoleClaim, user.Role.ToString()),
        };
        var identity = new ClaimsIdentity(claims, AdminAuth.Scheme);
        var ctx = http.HttpContext ?? throw new InvalidOperationException("No HttpContext.");
        await ctx.SignInAsync(AdminAuth.Scheme, new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) });
    }

    public async Task SignOutAsync()
    {
        var ctx = http.HttpContext ?? throw new InvalidOperationException("No HttpContext.");
        await ctx.SignOutAsync(AdminAuth.Scheme);
    }

    /// <summary>The signed-in admin's id, or null if the request isn't from an admin.</summary>
    public int? CurrentAdminId()
    {
        var claim = http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        // Only trust the claim when it came from the admin cookie, not the customer one.
        var isAdmin = http.HttpContext?.User.Identity is ClaimsIdentity id
            && id.AuthenticationType == AdminAuth.Scheme;
        return isAdmin && int.TryParse(claim?.Value, out var v) ? v : null;
    }

    /// <summary>Records a write to the audit trail. Callers still SaveChanges themselves.</summary>
    public void Audit(string action, string entityType, int? entityId = null, string? detail = null)
    {
        db.AdminAuditLogs.Add(new AdminAuditLog
        {
            AdminUserId = CurrentAdminId(),
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Detail = detail,
            CreatedAt = DateTime.UtcNow,
        });
    }

    /// <summary>
    /// Creates the first Owner account on startup if the table is empty, so the panel is
    /// reachable out of the box. Credentials come from config (Admin:Email / Admin:Password),
    /// falling back to a dev default that MUST be changed before production.
    /// </summary>
    public static async Task EnsureSeedAdminAsync(HoaiiDbContext db, IConfiguration config)
    {
        if (await db.AdminUsers.AnyAsync())
        {
            return;
        }

        var email = config["Admin:Email"] ?? "admin@hoaii.vn";
        var password = config["Admin:Password"] ?? "Hoaii@2026";

        var owner = new AdminUser
        {
            Email = email,
            FullName = "Chủ shop",
            Role = AdminRole.Owner,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            PasswordHash = "",
        };
        owner.PasswordHash = Hasher.HashPassword(owner, password);
        db.AdminUsers.Add(owner);
        await db.SaveChangesAsync();
    }
}
