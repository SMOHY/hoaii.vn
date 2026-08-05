using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Services;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Areas.Admin.Controllers;

/// <summary>Model for Areas/Admin/Views/Shared/_DestPicker.cshtml — a plain class rather than a
/// tuple because Razor's @model directive parser trips over a tuple nested inside a tuple.</summary>
public class DestPickerModel
{
    public required List<Category> Categories { get; init; }
    public required List<PolicyPage> Policies { get; init; }
    public required (string Key, string Label, string Url)[] StaticPages { get; init; }
    public string? Selected { get; init; }
}

/// <summary>Edits the header menus (main + sub) and the footer columns/links. Every write drops
/// the NavigationService cache so the storefront updates immediately.</summary>
public class MenuController(HoaiiDbContext db, AdminAuthService auth, NavigationService nav, PageContentService content) : BaseAdminController(db)
{
    /// <summary>Every internal page an admin might want to link to that isn't a Category. A
    /// fixed, known-good list rather than free text — the "Quà theo dịp" mega-menu bug (an admin
    /// typed a URL one character off from the real route and nothing on the site caught it) only
    /// happened because the old form let anyone type any string. Picking from this list can't
    /// produce a URL that doesn't exist.</summary>
    private static readonly (string Key, string Label, string Url)[] StaticPages =
    [
        ("trang-chu", "Trang chủ", "/"),
        ("lien-he", "Liên hệ", "/lien-he"),
        ("ve-chung-toi", "Về chúng tôi", "/ve-chung-toi"),
        ("hop-tac", "Đối tác / Hợp tác", "/hop-tac"),
        ("blog", "Blog", "/blog"),
        ("qua-theo-dip", "Quà theo dịp (trang landing)", "/qua-theo-dip"),
        ("qua-tang-ca-nhan", "Quà tặng cá nhân (trang landing)", "/qua-tang-ca-nhan"),
    ];

    /// <summary>"cat:5" / "page:blog" from the destination picker → the real URL + a human label
    /// for the audit log. Never trusts a URL typed by hand.</summary>
    private async Task<(string Url, string Label)?> ResolveDestinationAsync(string? dest)
    {
        if (string.IsNullOrWhiteSpace(dest)) return null;
        var parts = dest.Split(':', 2);
        if (parts.Length != 2) return null;

        if (parts[0] == "cat")
        {
            if (!int.TryParse(parts[1], out var catId)) return null;
            var cat = await Db.Categories.FindAsync(catId);
            return cat is null ? null : ($"/danh-muc/{cat.Slug}", cat.Name);
        }
        if (parts[0] == "page")
        {
            var page = StaticPages.FirstOrDefault(p => p.Key == parts[1]);
            return page.Key is null ? null : (page.Url, page.Label);
        }
        if (parts[0] == "policy")
        {
            var policy = await Db.PolicyPages.FirstOrDefaultAsync(p => p.Slug == parts[1]);
            return policy is null ? null : ($"/chinh-sach/{policy.Slug}", policy.NavLabel);
        }
        return null;
    }

    /// <summary>The reverse of <see cref="ResolveDestinationAsync"/> — given a URL already saved
    /// on a link, which option in the picker should show as selected. Falls back to null (picker
    /// shows "chưa gán" and the admin has to actively repoint it) for the rare URL that predates
    /// this picker and matches nothing — better than silently guessing wrong.</summary>
    private static string? ComputeDestKey(string url, List<Category> categories, List<PolicyPage> policies)
    {
        var page = StaticPages.FirstOrDefault(p => p.Url == url);
        if (page.Key is not null) return $"page:{page.Key}";

        var catMatch = categories.FirstOrDefault(c => $"/danh-muc/{c.Slug}" == url);
        if (catMatch is not null) return $"cat:{catMatch.Id}";

        var policyMatch = policies.FirstOrDefault(p => $"/chinh-sach/{p.Slug}" == url);
        return policyMatch is not null ? $"policy:{policyMatch.Slug}" : null;
    }

    private async Task LoadDestinationOptionsAsync()
    {
        // "ruou" (alcohol) is a conditional business line held back from the storefront until
        // the retail licence is in hand — same reason MegaMenuColumnMigrationSeeder never seeded
        // it into "Sản phẩm" and it must not be pickable as a destination/category-link here
        // either, or an admin could wire it back into the public nav by hand.
        var categories = await Db.Categories.Where(c => c.Slug != "ruou").OrderBy(c => c.Type).ThenBy(c => c.SortOrder).ThenBy(c => c.Id).ToListAsync();
        var policies = await Db.PolicyPages.OrderBy(p => p.SortOrder).ToListAsync();
        ViewBag.Categories = categories;
        ViewBag.Policies = policies;
        ViewBag.StaticPages = StaticPages;
        ViewBag.DestKeyFor = (Func<string, string?>)(url => ComputeDestKey(url, categories, policies));
    }

    /// <summary>The 8 product columns across the 4 built-in panels that the client wants
    /// hand-picked rather than auto-ranked by IsFeatured/Badge/age (no real sales data exists to
    /// rank "bán chạy nhất" by). Keys match MegaMenuViewComponent's PanelKey/ResolveColumnAsync
    /// calls exactly — this list is only for the admin UI, it doesn't drive the storefront.</summary>
    public static readonly (string PanelKey, string PanelLabel, string ColumnKey, string ColumnLabel)[] CuratedSlots =
    [
        ("qua-tet", "Quà tết", "best-sellers", "Bán chạy nhất"),
        ("qua-tet", "Quà tết", "limited", "Phiên bản giới hạn"),
        ("qua-trung-thu", "Quà trung thu", "best-sellers", "Bán chạy nhất"),
        ("qua-trung-thu", "Quà trung thu", "limited", "Phiên bản giới hạn"),
        ("qua-theo-dip", "Quà theo dịp", "suggested", "Hoài gợi ý"),
        ("qua-theo-dip", "Quà theo dịp", "best-sellers", "Bán chạy nhất"),
        ("san-pham-chon-loc", "Sản phẩm chọn lọc", "best-sellers", "Bán chạy nhất"),
        ("san-pham-chon-loc", "Sản phẩm chọn lọc", "featured", "Nổi bật"),
    ];

    [HttpGet("/admin/menu")]
    public async Task<IActionResult> Index()
    {
        ViewBag.TextFields = PageContentKeys.ForPage(PageContentKeys.Footer);
        ViewBag.TextValues = content.GetForEditing(PageContentKeys.Footer);
        ViewBag.Main = await Db.NavLinks.Where(l => l.Placement == NavPlacement.Main && l.ParentId == null)
            .Include(l => l.Children.OrderBy(c => c.SortOrder))
            .OrderBy(l => l.SortOrder).ThenBy(l => l.Id).ToListAsync();
        ViewBag.Sub = await Db.NavLinks.Where(l => l.Placement == NavPlacement.Sub).OrderBy(l => l.SortOrder).ThenBy(l => l.Id).ToListAsync();
        ViewBag.Columns = await Db.FooterMenuColumns.Include(c => c.Links.OrderBy(l => l.SortOrder)).OrderBy(c => c.SortOrder).ThenBy(c => c.Id).ToListAsync();
        await LoadDestinationOptionsAsync();

        ViewBag.CustomColumns = await Db.MegaMenuColumns
            .Include(c => c.Collection)
            .Include(c => c.Items).ThenInclude(i => i.Product)
            .Include(c => c.Items).ThenInclude(i => i.Category)
            .OrderBy(c => c.PanelKey).ThenBy(c => c.SortOrder)
            .ToListAsync();
        ViewBag.CollectionOptions = await Db.Collections.OrderBy(c => c.SortOrder).ToListAsync();

        // "Quà tặng" của panel Quà theo dịp giờ sửa ngay tại đây thay vì phải qua trang Danh mục.
        ViewBag.CategoryGroups = await Db.CategoryGroups
            .Include(g => g.Categories.OrderBy(c => c.SortOrder))
            .OrderBy(g => g.SortOrder)
            .ToListAsync();
        ViewBag.OccasionCategories = await Db.Categories
            .Where(c => c.Type == CategoryType.Occasion)
            .OrderBy(c => c.SortOrder)
            .ToListAsync();

        return View();
    }

    // ---------- Custom mega-menu columns (beyond the built-in 8 slots) ----------
    [HttpPost("/admin/menu/cot-tuy-chinh/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CustomColumnSave(int id, string panelKey, string title, MegaMenuColumnKind kind, int sortOrder, int? collectionId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            Fail("Tên cột không được để trống.");
            return RedirectToAction(nameof(Index));
        }
        var x = id == 0
            ? new MegaMenuColumn { PanelKey = panelKey, Title = "" }
            : await Db.MegaMenuColumns.FindAsync(id);
        if (x is null) return NotFound();
        x.Title = title.Trim();
        x.Kind = kind;
        x.SortOrder = sortOrder;
        x.CollectionId = kind == MegaMenuColumnKind.Collection ? collectionId : null;
        if (id == 0) Db.MegaMenuColumns.Add(x);
        auth.Audit(id == 0 ? "Thêm cột dropdown" : "Sửa cột dropdown", nameof(MegaMenuColumn), id == 0 ? null : id, title);
        await Db.SaveChangesAsync();
        Done("Đã lưu cột.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/menu/cot-tuy-chinh/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CustomColumnDelete(int id)
    {
        var x = await Db.MegaMenuColumns.FindAsync(id);
        if (x is null) return NotFound();
        Db.MegaMenuColumns.Remove(x); // items cascade
        auth.Audit("Xóa cột dropdown", nameof(MegaMenuColumn), id, x.Title);
        await Db.SaveChangesAsync();
        Done("Đã xóa cột.");
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Replaces the whole item set for one custom column — same "always resend the full
    /// list" pattern as CuratedSave. productIds is used when Kind == Pick, categoryIds when
    /// Kind == CategoryLinks; the one that doesn't apply to this column just arrives empty.</summary>
    [HttpPost("/admin/menu/cot-tuy-chinh/{id:int}/items")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CustomColumnItemsSave(int id, List<int> productIds, List<int> categoryIds)
    {
        var column = await Db.MegaMenuColumns.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);
        if (column is null) return NotFound();

        Db.MegaMenuColumnItems.RemoveRange(column.Items);
        var order = 0;
        if (column.Kind == MegaMenuColumnKind.Pick)
        {
            foreach (var productId in productIds.Distinct().Take(4))
            {
                Db.MegaMenuColumnItems.Add(new MegaMenuColumnItem { MegaMenuColumnId = id, ProductId = productId, SortOrder = order++ });
            }
        }
        else if (column.Kind == MegaMenuColumnKind.CategoryLinks)
        {
            // Category rows have no thumbnail, so this kind tolerates more than the 4-pick cap
            // Pick/Collection use — matches the Take(6) in MegaMenuViewComponent's render.
            foreach (var categoryId in categoryIds.Distinct().Take(6))
            {
                Db.MegaMenuColumnItems.Add(new MegaMenuColumnItem { MegaMenuColumnId = id, CategoryId = categoryId, SortOrder = order++ });
            }
        }
        auth.Audit("Sửa nội dung cột dropdown", nameof(MegaMenuColumn), id, column.Title);
        await Db.SaveChangesAsync();
        Done("Đã lưu nội dung cột.");
        return RedirectToAction(nameof(Index));
    }

    // ---------- Curated product picks (8 slots — see CuratedSlots) ----------
    [HttpGet("/admin/menu/tim-san-pham")]
    public async Task<IActionResult> SearchProducts(string? q)
    {
        var query = Db.Products.Include(p => p.Images).Include(p => p.Category).Where(p => p.IsActive);
        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(p => p.Name.Contains(term));
        }
        var items = await query.OrderBy(p => p.Name).Take(20)
            .Select(p => new
            {
                p.Id,
                p.Name,
                category = p.Category.Name,
                imageUrl = p.Images.OrderBy(i => i.SortOrder).Select(i => i.Url).FirstOrDefault(),
            })
            .ToListAsync();
        return Json(items);
    }

    private void Done(string msg)
    {
        nav.Invalidate();
        Ok(msg);
    }

    [HttpPost("/admin/menu/nhan-tin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveNewsletter(Dictionary<string, string?> f)
    {
        await content.SaveAsync(PageContentKeys.Footer, f.ToDictionary(kv => kv.Key, kv => kv.Value));
        auth.Audit("Sửa khối nhận tin", nameof(PageContent));
        await Db.SaveChangesAsync();
        Ok("Đã lưu khối đăng ký nhận tin.");
        return RedirectToAction(nameof(Index));
    }

    // ---------- Header links ----------
    [HttpPost("/admin/menu/lien-ket/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LinkSave(int id, NavPlacement placement, string label, string dest, bool hasDropdown, int sortOrder)
    {
        var resolved = await ResolveDestinationAsync(dest);
        if (string.IsNullOrWhiteSpace(label) || resolved is null)
        {
            Fail("Nhãn không được để trống và phải chọn 1 đích đến hợp lệ.");
            return RedirectToAction(nameof(Index));
        }
        var x = id == 0 ? new NavLink { Label = "", Url = "" } : await Db.NavLinks.FindAsync(id);
        if (x is null) return NotFound();
        x.Placement = placement;
        x.Label = label.Trim();
        x.Url = resolved.Value.Url;
        x.HasDropdown = hasDropdown;
        x.SortOrder = sortOrder;
        if (id == 0) Db.NavLinks.Add(x);
        auth.Audit(id == 0 ? "Thêm liên kết menu" : "Sửa liên kết menu", nameof(NavLink), id == 0 ? null : id, label);
        await Db.SaveChangesAsync();
        Done("Đã lưu liên kết.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/menu/lien-ket/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LinkDelete(int id)
    {
        // ParentId → self is Restrict at the DB level (SQL Server won't allow a cascading FK on a
        // self-reference), so any submenu items have to go first or this throws a FK violation.
        var x = await Db.NavLinks.Include(l => l.Children).FirstOrDefaultAsync(l => l.Id == id);
        if (x is null) return NotFound();
        Db.NavLinks.RemoveRange(x.Children);
        Db.NavLinks.Remove(x);
        auth.Audit("Xóa liên kết menu", nameof(NavLink), id, x.Label);
        await Db.SaveChangesAsync();
        Done("Đã xóa liên kết.");
        return RedirectToAction(nameof(Index));
    }

    // ---------- Submenu (dropdown children of a Main link) ----------
    [HttpPost("/admin/menu/menu-con/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubLinkSave(int id, int parentId, string label, string dest, int sortOrder)
    {
        var resolved = await ResolveDestinationAsync(dest);
        if (string.IsNullOrWhiteSpace(label) || resolved is null)
        {
            Fail("Nhãn không được để trống và phải chọn 1 đích đến hợp lệ.");
            return RedirectToAction(nameof(Index));
        }
        var parent = await Db.NavLinks.FindAsync(parentId);
        if (parent is null || parent.Placement != NavPlacement.Main || parent.ParentId is not null)
        {
            return NotFound();
        }
        var x = id == 0 ? new NavLink { Label = "", Url = "", Placement = NavPlacement.Main, ParentId = parentId } : await Db.NavLinks.FindAsync(id);
        if (x is null) return NotFound();
        x.Label = label.Trim();
        x.Url = resolved.Value.Url;
        x.SortOrder = sortOrder;
        if (id == 0) Db.NavLinks.Add(x);
        auth.Audit(id == 0 ? "Thêm mục menu con" : "Sửa mục menu con", nameof(NavLink), id == 0 ? null : id, label);
        await Db.SaveChangesAsync();
        Done("Đã lưu mục menu con.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/menu/menu-con/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubLinkDelete(int id)
    {
        var x = await Db.NavLinks.FindAsync(id);
        if (x is null) return NotFound();
        Db.NavLinks.Remove(x);
        auth.Audit("Xóa mục menu con", nameof(NavLink), id, x.Label);
        await Db.SaveChangesAsync();
        Done("Đã xóa mục menu con.");
        return RedirectToAction(nameof(Index));
    }

    // ---------- Footer columns ----------
    [HttpPost("/admin/menu/cot/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ColumnSave(int id, string title, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            Fail("Tiêu đề cột không được để trống.");
            return RedirectToAction(nameof(Index));
        }
        var x = id == 0 ? new FooterMenuColumn { Title = "" } : await Db.FooterMenuColumns.FindAsync(id);
        if (x is null) return NotFound();
        x.Title = title.Trim();
        x.SortOrder = sortOrder;
        if (id == 0) Db.FooterMenuColumns.Add(x);
        auth.Audit(id == 0 ? "Thêm cột footer" : "Sửa cột footer", nameof(FooterMenuColumn), id == 0 ? null : id, title);
        await Db.SaveChangesAsync();
        Done("Đã lưu cột.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/menu/cot/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ColumnDelete(int id)
    {
        var x = await Db.FooterMenuColumns.FindAsync(id);
        if (x is null) return NotFound();
        Db.FooterMenuColumns.Remove(x); // links cascade
        auth.Audit("Xóa cột footer", nameof(FooterMenuColumn), id, x.Title);
        await Db.SaveChangesAsync();
        Done("Đã xóa cột.");
        return RedirectToAction(nameof(Index));
    }

    // ---------- Footer links ----------
    [HttpPost("/admin/menu/cot-lien-ket/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ColumnLinkSave(int id, int columnId, string label, string dest, int sortOrder)
    {
        var resolved = await ResolveDestinationAsync(dest);
        if (string.IsNullOrWhiteSpace(label) || resolved is null)
        {
            Fail("Nhãn không được để trống và phải chọn 1 đích đến hợp lệ.");
            return RedirectToAction(nameof(Index));
        }
        var x = id == 0 ? new FooterMenuLink { Label = "", Url = "", FooterMenuColumnId = columnId } : await Db.FooterMenuLinks.FindAsync(id);
        if (x is null) return NotFound();
        x.Label = label.Trim();
        x.Url = resolved.Value.Url;
        x.SortOrder = sortOrder;
        if (id == 0) Db.FooterMenuLinks.Add(x);
        auth.Audit(id == 0 ? "Thêm link footer" : "Sửa link footer", nameof(FooterMenuLink), id == 0 ? null : id, label);
        await Db.SaveChangesAsync();
        Done("Đã lưu liên kết footer.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/menu/cot-lien-ket/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ColumnLinkDelete(int id)
    {
        var x = await Db.FooterMenuLinks.FindAsync(id);
        if (x is null) return NotFound();
        Db.FooterMenuLinks.Remove(x);
        auth.Audit("Xóa link footer", nameof(FooterMenuLink), id, x.Label);
        await Db.SaveChangesAsync();
        Done("Đã xóa liên kết footer.");
        return RedirectToAction(nameof(Index));
    }
}
