using System.Text.RegularExpressions;

namespace Hoaii.Web.Services;

/// <summary>
/// The focal point an admin picks on a CMS image, stored as a CSS object-position pair such as
/// "50% 30%" and written straight into a style attribute on the storefront. Because it reaches
/// the page as CSS, it is validated in exactly one place — anything that is not a pair of
/// percentages is dropped rather than rendered, so a hand-edited row cannot smuggle in styling.
/// </summary>
public static partial class FocalPoint
{
    [GeneratedRegex(@"^-?\d{1,3}(\.\d+)?% -?\d{1,3}(\.\d+)?%$")]
    private static partial Regex Pair();

    /// <summary>The stored value, or null when it is missing or malformed.</summary>
    public static string? Sanitise(string? value)
    {
        var v = value?.Trim();
        return !string.IsNullOrEmpty(v) && Pair().IsMatch(v) ? v : null;
    }

    /// <summary>Inline style for an img, or null to leave the element's own CSS alone. Centre is
    /// the browser default for object-fit, so it is deliberately not emitted.</summary>
    public static string? Style(string? value) =>
        Sanitise(value) is { } v && v != "50% 50%" ? $"object-position:{v}" : null;
}
