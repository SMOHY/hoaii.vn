namespace Hoaii.Web.Areas.Admin.Models;

/// <summary>
/// One image slot in a CMS form. A slot is always a desktop image; it optionally carries a
/// mobile override and a focal point. Both extras are opt-in per call site so small images
/// (icons, partner logos) keep the plain one-input field they had before — the point of the
/// tabbed UI is to stop doubling the number of visible fields, not to put a tab bar on
/// every icon in the admin.
/// </summary>
/// <param name="Label">Field label shown above the block.</param>
/// <param name="Name">Form field name for the desktop image path.</param>
/// <param name="Id">DOM id — also what the shared media picker targets.</param>
/// <param name="Value">Current desktop image path.</param>
public sealed record ImageFieldModel(string Label, string Name, string Id, string? Value)
{
    /// <summary>Form field name for the mobile override. Null means this slot has no mobile tab.</summary>
    public string? MobileName { get; init; }

    public string? MobileId { get; init; }

    /// <summary>Mobile image path. Empty/null means "share the desktop image" — the storefront
    /// renders a plain img in that case, so an untouched slot behaves exactly as before.</summary>
    public string? MobileValue { get; init; }

    /// <summary>Form field name for the focal point. Null means this slot has no focal picker.</summary>
    public string? FocalName { get; init; }

    public string? FocalId { get; init; }

    /// <summary>Focal point as a CSS object-position pair, e.g. "50% 32%". Empty means centre.</summary>
    public string? FocalValue { get; init; }

    public bool HasMobile => !string.IsNullOrEmpty(MobileName);

    public bool HasFocal => !string.IsNullOrEmpty(FocalName);
}
