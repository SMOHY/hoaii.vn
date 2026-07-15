namespace Hoaii.Domain.Entities;

/// <summary>
/// A single editable site-wide value (hotline, Zalo number, address, social URLs…). These were
/// scattered as string literals across a dozen views — the nav hotline and the contact-page
/// phone number even disagreed with each other. Key/value keeps the shape simple; the known
/// keys live in SiteSettingKeys.
/// </summary>
public class SiteSetting
{
    public required string Key { get; set; }
    public string Value { get; set; } = "";
}
