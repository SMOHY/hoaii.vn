namespace Hoaii.Web.Models.Layout;

public class NavMenuItem
{
    public required string Label { get; init; }
    public required string Url { get; init; }
    public bool HasDropdown { get; init; }
}
