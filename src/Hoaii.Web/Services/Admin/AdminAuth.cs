namespace Hoaii.Web.Services.Admin;

/// <summary>Names shared between Program.cs, the admin controllers and the auth service.</summary>
public static class AdminAuth
{
    /// <summary>Second cookie scheme, kept apart from the customer "Cookies" scheme so signing
    /// out of one never touches the other.</summary>
    public const string Scheme = "AdminCookie";

    /// <summary>Any signed-in admin.</summary>
    public const string PolicyAdmin = "AdminOnly";

    /// <summary>Owner only — deleting products, managing admins, payment settings.</summary>
    public const string PolicyOwner = "OwnerOnly";

    public const string RoleClaim = "hoaii_admin_role";
}
