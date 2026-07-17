using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Services;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Areas.Admin.Controllers;

/// <summary>
/// Edits the six homepage sections. One controller, one dashboard, a small edit form per section
/// type. Customer logos are simple enough to manage inline on the dashboard.
/// </summary>
public class HomepageController(HoaiiDbContext db, AdminAuthService auth, PageContentService content) : BaseAdminController(db)
{
    [HttpGet("/admin/trang-chu")]
    public async Task<IActionResult> Index()
    {
        // Section headings/intros live in the reusable PageContent store.
        ViewBag.TextFields = PageContentKeys.ForPage(PageContentKeys.Home);
        ViewBag.TextValues = content.GetForEditing(PageContentKeys.Home);
        ViewBag.Heroes = await Db.HomeHeroSlides.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync();
        ViewBag.Benefits = await Db.HomeBenefits.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync();
        ViewBag.Tiles = await Db.HomeFeaturedTiles.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync();
        ViewBag.Services = await Db.HomeServiceTabs.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync();
        ViewBag.Abouts = await Db.HomeAboutCards.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync();
        ViewBag.Logos = await Db.HomeCustomerLogos.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync();
        return View();
    }

    [HttpPost("/admin/trang-chu/chu-khung")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveText(Dictionary<string, string?> f)
    {
        await content.SaveAsync(PageContentKeys.Home, f.ToDictionary(kv => kv.Key, kv => kv.Value));
        auth.Audit("Sửa chữ khung trang chủ", nameof(PageContent));
        await Db.SaveChangesAsync();
        Ok("Đã lưu tiêu đề các mục.");
        return RedirectToAction(nameof(Index));
    }

    // ---------- Hero ----------
    [HttpGet("/admin/trang-chu/hero/them")]
    public IActionResult HeroCreate() => View("HeroEdit", new HomeHeroSlide { ImageUrl = "" });

    [HttpGet("/admin/trang-chu/hero/{id:int}/sua")]
    public async Task<IActionResult> HeroEdit(int id)
    {
        var x = await Db.HomeHeroSlides.FindAsync(id);
        return x is null ? NotFound() : View(x);
    }

    [HttpPost("/admin/trang-chu/hero/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HeroSave(int id, string imageUrl, string? title, string? subtitle, string? mobileTitle, string? mobileSubtitle, int sortOrder, bool isActive)
    {
        var x = id == 0 ? new HomeHeroSlide { ImageUrl = "" } : await Db.HomeHeroSlides.FindAsync(id);
        if (x is null) return NotFound();
        x.ImageUrl = imageUrl?.Trim() ?? "";
        x.Title = title?.Trim() ?? "";
        x.Subtitle = subtitle?.Trim() ?? "";
        x.MobileTitle = mobileTitle?.Trim() ?? "";
        x.MobileSubtitle = mobileSubtitle?.Trim() ?? "";
        x.SortOrder = sortOrder;
        x.IsActive = isActive;
        if (id == 0) Db.HomeHeroSlides.Add(x);
        auth.Audit(id == 0 ? "Thêm hero" : "Sửa hero", nameof(HomeHeroSlide), id == 0 ? null : id);
        await Db.SaveChangesAsync();
        Ok("Đã lưu ảnh hero.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/trang-chu/hero/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> HeroDelete(int id) => DeleteAsync(Db.HomeHeroSlides, id, "hero");

    // ---------- Benefit ----------
    [HttpGet("/admin/trang-chu/loi-ich/them")]
    public IActionResult BenefitCreate() => View("BenefitEdit", new HomeBenefit());

    [HttpGet("/admin/trang-chu/loi-ich/{id:int}/sua")]
    public async Task<IActionResult> BenefitEdit(int id)
    {
        var x = await Db.HomeBenefits.FindAsync(id);
        return x is null ? NotFound() : View(x);
    }

    [HttpPost("/admin/trang-chu/loi-ich/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BenefitSave(int id, string? iconPath, string? title, string? description, string? mobileLine1, string? mobileLine2, int sortOrder)
    {
        var x = id == 0 ? new HomeBenefit() : await Db.HomeBenefits.FindAsync(id);
        if (x is null) return NotFound();
        x.IconPath = iconPath?.Trim() ?? "";
        x.Title = title?.Trim() ?? "";
        x.Description = description?.Trim() ?? "";
        x.MobileLine1 = mobileLine1?.Trim() ?? "";
        x.MobileLine2 = mobileLine2?.Trim() ?? "";
        x.SortOrder = sortOrder;
        if (id == 0) Db.HomeBenefits.Add(x);
        auth.Audit(id == 0 ? "Thêm lợi ích" : "Sửa lợi ích", nameof(HomeBenefit), id == 0 ? null : id);
        await Db.SaveChangesAsync();
        Ok("Đã lưu lợi ích.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/trang-chu/loi-ich/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> BenefitDelete(int id) => DeleteAsync(Db.HomeBenefits, id, "lợi ích");

    // ---------- Featured tile ----------
    [HttpGet("/admin/trang-chu/o-noi-bat/them")]
    public IActionResult TileCreate() => View("TileEdit", new HomeFeaturedTile());

    [HttpGet("/admin/trang-chu/o-noi-bat/{id:int}/sua")]
    public async Task<IActionResult> TileEdit(int id)
    {
        var x = await Db.HomeFeaturedTiles.FindAsync(id);
        return x is null ? NotFound() : View(x);
    }

    [HttpPost("/admin/trang-chu/o-noi-bat/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TileSave(int id, bool isCard, string? accentColor, string? collectionLabel, string? titleLine1, string? titleLine2, string? editionLabel, bool hideOnMobile, string? imageUrl, string? linkUrl, int sortOrder)
    {
        var x = id == 0 ? new HomeFeaturedTile() : await Db.HomeFeaturedTiles.FindAsync(id);
        if (x is null) return NotFound();
        x.IsCard = isCard;
        x.AccentColor = string.IsNullOrWhiteSpace(accentColor) ? null : accentColor.Trim();
        x.CollectionLabel = collectionLabel?.Trim();
        x.TitleLine1 = titleLine1?.Trim();
        x.TitleLine2 = titleLine2?.Trim();
        x.EditionLabel = editionLabel?.Trim();
        x.HideOnMobile = hideOnMobile;
        x.ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl.Trim();
        x.LinkUrl = string.IsNullOrWhiteSpace(linkUrl) ? "#" : linkUrl.Trim();
        x.SortOrder = sortOrder;
        if (id == 0) Db.HomeFeaturedTiles.Add(x);
        auth.Audit(id == 0 ? "Thêm ô nổi bật" : "Sửa ô nổi bật", nameof(HomeFeaturedTile), id == 0 ? null : id);
        await Db.SaveChangesAsync();
        Ok("Đã lưu ô nổi bật.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/trang-chu/o-noi-bat/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> TileDelete(int id) => DeleteAsync(Db.HomeFeaturedTiles, id, "ô nổi bật");

    // ---------- Service tab ----------
    [HttpGet("/admin/trang-chu/dich-vu/them")]
    public IActionResult ServiceCreate() => View("ServiceEdit", new HomeServiceTab { Key = "" });

    [HttpGet("/admin/trang-chu/dich-vu/{id:int}/sua")]
    public async Task<IActionResult> ServiceEdit(int id)
    {
        var x = await Db.HomeServiceTabs.FindAsync(id);
        return x is null ? NotFound() : View(x);
    }

    [HttpPost("/admin/trang-chu/dich-vu/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ServiceSave(int id, string key, string? label, string? iconSvg, string? panelImageUrl, string? caption, string? captionColorHex, string? ctaText, string? ctaUrl, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Fail("Mã (key) không được để trống.");
            return RedirectToAction(id == 0 ? nameof(ServiceCreate) : nameof(ServiceEdit), id == 0 ? null : new { id });
        }
        var x = id == 0 ? new HomeServiceTab { Key = "" } : await Db.HomeServiceTabs.FindAsync(id);
        if (x is null) return NotFound();
        x.Key = key.Trim();
        x.Label = label?.Trim() ?? "";
        x.IconSvg = iconSvg?.Trim() ?? "";
        x.PanelImageUrl = panelImageUrl?.Trim() ?? "";
        x.Caption = caption?.Trim() ?? "";
        x.CaptionColorHex = string.IsNullOrWhiteSpace(captionColorHex) ? "#F2F2F2" : captionColorHex.Trim();
        x.CtaText = string.IsNullOrWhiteSpace(ctaText) ? "Bắt đầu" : ctaText.Trim();
        x.CtaUrl = string.IsNullOrWhiteSpace(ctaUrl) ? "#" : ctaUrl.Trim();
        x.SortOrder = sortOrder;
        if (id == 0) Db.HomeServiceTabs.Add(x);
        auth.Audit(id == 0 ? "Thêm dịch vụ" : "Sửa dịch vụ", nameof(HomeServiceTab), id == 0 ? null : id);
        await Db.SaveChangesAsync();
        Ok("Đã lưu dịch vụ.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/trang-chu/dich-vu/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> ServiceDelete(int id) => DeleteAsync(Db.HomeServiceTabs, id, "dịch vụ");

    // ---------- About card ----------
    [HttpGet("/admin/trang-chu/gia-tri/them")]
    public IActionResult AboutCreate() => View("AboutEdit", new HomeAboutCard());

    [HttpGet("/admin/trang-chu/gia-tri/{id:int}/sua")]
    public async Task<IActionResult> AboutEdit(int id)
    {
        var x = await Db.HomeAboutCards.FindAsync(id);
        return x is null ? NotFound() : View(x);
    }

    [HttpPost("/admin/trang-chu/gia-tri/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AboutSave(int id, string? caption, bool imageOnTop, string? imageUrl, int sortOrder)
    {
        var x = id == 0 ? new HomeAboutCard() : await Db.HomeAboutCards.FindAsync(id);
        if (x is null) return NotFound();
        x.Caption = caption?.Trim() ?? "";
        x.ImageOnTop = imageOnTop;
        x.ImageUrl = imageUrl?.Trim() ?? "";
        x.SortOrder = sortOrder;
        if (id == 0) Db.HomeAboutCards.Add(x);
        auth.Audit(id == 0 ? "Thêm giá trị" : "Sửa giá trị", nameof(HomeAboutCard), id == 0 ? null : id);
        await Db.SaveChangesAsync();
        Ok("Đã lưu thẻ giá trị.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/trang-chu/gia-tri/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> AboutDelete(int id) => DeleteAsync(Db.HomeAboutCards, id, "thẻ giá trị");

    // ---------- Customer logos (managed inline on the dashboard) ----------
    [HttpPost("/admin/trang-chu/logo/them")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogoAdd(string logoKey)
    {
        if (!string.IsNullOrWhiteSpace(logoKey))
        {
            var max = await Db.HomeCustomerLogos.MaxAsync(l => (int?)l.SortOrder) ?? -1;
            Db.HomeCustomerLogos.Add(new HomeCustomerLogo { LogoKey = logoKey.Trim(), SortOrder = max + 1 });
            auth.Audit("Thêm logo KH", nameof(HomeCustomerLogo), null, logoKey);
            await Db.SaveChangesAsync();
            Ok("Đã thêm logo.");
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/trang-chu/logo/{id:int}/xoa")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> LogoDelete(int id) => DeleteAsync(Db.HomeCustomerLogos, id, "logo");

    private async Task<IActionResult> DeleteAsync<T>(DbSet<T> set, int id, string label) where T : class
    {
        var x = await set.FindAsync(id);
        if (x is null) return NotFound();
        set.Remove(x);
        auth.Audit($"Xóa {label}", typeof(T).Name, id);
        await Db.SaveChangesAsync();
        Ok($"Đã xóa {label}.");
        return RedirectToAction(nameof(Index));
    }
}
