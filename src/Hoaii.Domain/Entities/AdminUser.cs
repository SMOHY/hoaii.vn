namespace Hoaii.Domain.Entities;

public enum AdminRole
{
    /// <summary>Full access, including deleting products and managing other admins.</summary>
    Owner,

    /// <summary>Day-to-day: orders, products, content. Cannot delete or manage admins.</summary>
    Staff,
}

/// <summary>
/// Deliberately separate from <see cref="Customer"/>. Customers sign in with an emailed OTP,
/// which today only writes the code to the log — an admin who depended on that would be locked
/// out until a mail provider is wired up. Admins get their own password and their own cookie.
/// </summary>
public class AdminUser
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string FullName { get; set; }

    /// <summary>Hashed with Microsoft.AspNetCore.Identity's PasswordHasher — never plain text.</summary>
    public required string PasswordHash { get; set; }

    public AdminRole Role { get; set; } = AdminRole.Staff;
    public bool IsActive { get; set; } = true;

    // Brute-force guard: five wrong passwords locks the account for fifteen minutes.
    public int FailedAttempts { get; set; }
    public DateTime? LockedUntil { get; set; }

    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
