using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Services;

/// <summary>
/// Backs Areas/Admin/Views/Shared/_DestPicker.cshtml — "pick a destination instead of typing a
/// URL" ("cat:5" / "col:3" / "page:blog" / "policy:trao-doi", resolved to a real URL server-side).
/// Originally lived only in MenuController; HomepageController needs the exact same picker for
/// tile/service CTA links, so it moved here rather than being copy-pasted a second time.
/// </summary>
public class DestinationLinkService(HoaiiDbContext db)
{
    /// <summary>Every internal page an admin might want to link to that isn't a Category or
    /// Collection. A fixed, known-good list rather than free text — the "Quà theo dịp" mega-menu
    /// bug (an admin typed a URL one character off from the real route and nothing on the site
    /// caught it) only happened because the old form let anyone type any string. Picking from
    /// this list can't produce a URL that doesn't exist.</summary>
    public static readonly (string Key, string Label, string Url)[] StaticPages =
    [
        ("trang-chu", "Trang chủ", "/"),
        ("lien-he", "Liên hệ", "/lien-he"),
        ("ve-chung-toi", "Về chúng tôi", "/ve-chung-toi"),
        ("hop-tac", "Đối tác / Hợp tác", "/hop-tac"),
        ("blog", "Blog", "/blog"),
        ("qua-theo-dip", "Quà theo dịp (trang landing)", "/qua-theo-dip"),
        ("qua-tang-ca-nhan", "Quà tặng cá nhân (trang landing)", "/qua-tang-ca-nhan"),
    ];

    public record Options(List<Category> Categories, List<Collection> Collections, List<PolicyPage> Policies);

    public async Task<Options> LoadOptionsAsync()
    {
        // "ruou" (alcohol) is a conditional business line held back from the storefront until
        // the retail licence is in hand — must not be pickable as a link destination either, or
        // an admin could wire it back into public pages by hand.
        var categories = await db.Categories.Where(c => c.Slug != "ruou")
            .OrderBy(c => c.Type).ThenBy(c => c.SortOrder).ThenBy(c => c.Id).ToListAsync();
        var collections = await db.Collections.OrderBy(c => c.SortOrder).ThenBy(c => c.Id).ToListAsync();
        var policies = await db.PolicyPages.OrderBy(p => p.SortOrder).ToListAsync();
        return new Options(categories, collections, policies);
    }

    /// <summary>"cat:5" / "col:3" / "page:blog" from the destination picker → the real URL + a
    /// human label for the audit log. Never trusts a URL typed by hand.</summary>
    public async Task<(string Url, string Label)?> ResolveAsync(string? dest)
    {
        if (string.IsNullOrWhiteSpace(dest)) return null;
        var parts = dest.Split(':', 2);
        if (parts.Length != 2) return null;

        if (parts[0] == "cat")
        {
            if (!int.TryParse(parts[1], out var catId)) return null;
            var cat = await db.Categories.FindAsync(catId);
            return cat is null ? null : ($"/danh-muc/{cat.Slug}", cat.Name);
        }
        if (parts[0] == "col")
        {
            if (!int.TryParse(parts[1], out var colId)) return null;
            var col = await db.Collections.FindAsync(colId);
            return col is null ? null : ($"/bo-suu-tap/{col.Slug}", col.Name);
        }
        if (parts[0] == "page")
        {
            var page = StaticPages.FirstOrDefault(p => p.Key == parts[1]);
            return page.Key is null ? null : (page.Url, page.Label);
        }
        if (parts[0] == "policy")
        {
            var policy = await db.PolicyPages.FirstOrDefaultAsync(p => p.Slug == parts[1]);
            return policy is null ? null : ($"/chinh-sach/{policy.Slug}", policy.NavLabel);
        }
        return null;
    }

    /// <summary>The reverse of <see cref="ResolveAsync"/> — given a URL already saved on a link,
    /// which option in the picker should show as selected. Falls back to null (picker shows
    /// "chưa gán" and the admin has to actively repoint it) for the rare URL that predates this
    /// picker and matches nothing — better than silently guessing wrong.</summary>
    public static string? ComputeKey(string url, Options options)
    {
        var page = StaticPages.FirstOrDefault(p => p.Url == url);
        if (page.Key is not null) return $"page:{page.Key}";

        var catMatch = options.Categories.FirstOrDefault(c => $"/danh-muc/{c.Slug}" == url);
        if (catMatch is not null) return $"cat:{catMatch.Id}";

        var colMatch = options.Collections.FirstOrDefault(c => $"/bo-suu-tap/{c.Slug}" == url);
        if (colMatch is not null) return $"col:{colMatch.Id}";

        var policyMatch = options.Policies.FirstOrDefault(p => $"/chinh-sach/{p.Slug}" == url);
        return policyMatch is not null ? $"policy:{policyMatch.Slug}" : null;
    }
}
