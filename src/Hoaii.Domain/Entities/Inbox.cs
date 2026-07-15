namespace Hoaii.Domain.Entities;

/// <summary>A message from the storefront contact form.</summary>
public class ContactSubmission
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Phone { get; set; }
    public string Message { get; set; } = "";
    public bool IsHandled { get; set; }
    public DateTime CreatedAt { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();
}

/// <summary>A wholesale / corporate-gift enquiry from the partners page.</summary>
public class WholesaleLead
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Phone { get; set; }
    public string? PostalCode { get; set; }
    public string CompanyName { get; set; } = "";
    public string RequestType { get; set; } = "Business";
    public string? Message { get; set; }
    public bool IsHandled { get; set; }
    public DateTime CreatedAt { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();
}

/// <summary>A footer newsletter sign-up.</summary>
public class NewsletterSubscriber
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}
