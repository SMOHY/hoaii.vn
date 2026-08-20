namespace Hoaii.Web.Models;

/// <summary>
/// One storefront image that an admin may have customised for phones. Rendered by
/// Views/Shared/_ResponsiveImg.cshtml.
/// </summary>
/// <param name="Src">Desktop image path — the only required part.</param>
/// <param name="Alt">Alt text.</param>
public sealed record ResponsiveImage(string Src, string Alt)
{
    /// <summary>Replacement shown below 768px. Empty means "use the desktop image everywhere",
    /// which is the state every image starts in.</summary>
    public string? MobileSrc { get; init; }

    /// <summary>Focal point as a CSS object-position pair. Empty centres, as the browser does.</summary>
    public string? Focal { get; init; }

    public string? CssClass { get; init; }

    /// <summary>Value for the loading attribute; null omits it.</summary>
    public string? Loading { get; init; } = "lazy";

    /// <summary>Value for the fetchpriority attribute; null omits it.</summary>
    public string? FetchPriority { get; init; }
}
