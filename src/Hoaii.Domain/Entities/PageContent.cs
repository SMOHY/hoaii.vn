namespace Hoaii.Domain.Entities;

/// <summary>
/// A single editable piece of copy or image on an otherwise fixed-layout page (About, Partners…).
/// Keyed by page + block so the storefront reads a value with a coded fallback, and the admin edits
/// them in a grouped form. This is the reusable mechanism for "static" page content.
/// </summary>
public class PageContent
{
    public int Id { get; set; }
    public required string PageKey { get; set; }
    public required string BlockKey { get; set; }
    public string Value { get; set; } = "";
}

/// <summary>A partner logo on the Hợp tác page (basename of the asset under /images/partners/).</summary>
public class PartnerLogo
{
    public int Id { get; set; }
    public required string LogoKey { get; set; }
    public int SortOrder { get; set; }
}
