namespace Hoaii.Web.Services;

/// <summary>
/// Inline style for the few storefront blocks that show their image as a CSS background rather
/// than an img — the homepage hero and the two product-story panels. They cannot use a picture
/// element, so the mobile replacement travels as a custom property that a media query in the
/// stylesheet picks up; with no replacement set the property is absent and the rule never
/// matches, leaving the block exactly as it renders today.
/// </summary>
public static class CssImage
{
    /// <summary>A path is only emitted when it cannot break out of the url() it sits in — an
    /// admin-entered value reaches the page as CSS here, the same reason FocalPoint validates.</summary>
    private static bool IsSafePath(string? path) =>
        !string.IsNullOrWhiteSpace(path) && path.IndexOfAny(UnsafeChars) < 0;

    /// <summary>Quotes, brackets, backslash and the rule separator — the characters that
    /// would let a path escape the url() it is placed in.</summary>
    private static readonly char[] UnsafeChars = "'\"()\\;".ToCharArray();

    public static string Background(string? src, string? mobileSrc = null, string? focal = null)
    {
        var parts = new List<string>(3);
        if (IsSafePath(src)) parts.Add($"background-image:url('{src!.Trim()}')");
        if (IsSafePath(mobileSrc)) parts.Add($"--mobile-img:url('{mobileSrc!.Trim()}')");
        if (FocalPoint.Style(focal) is { } f) parts.Add(f);
        return string.Join(';', parts);
    }
}
