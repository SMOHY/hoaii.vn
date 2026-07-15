using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Areas.Admin.Models;
using Hoaii.Web.Services;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Areas.Admin.Controllers;

public class ProductsController(HoaiiDbContext db, AdminAuthService auth) : BaseAdminController(db)
{
    private const int PageSize = 20;

    [HttpGet("/admin/san-pham")]
    public async Task<IActionResult> Index(string? q, int? categoryId, int page = 1)
    {
        var query = Db.Products.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(p => p.Name.Contains(term) || p.Slug.Contains(term));
        }
        if (categoryId is not null)
        {
            query = query.Where(p => p.CategoryId == categoryId);
        }

        var total = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var rows = await query
            .OrderBy(p => p.CategoryId).ThenBy(p => p.SortOrder).ThenBy(p => p.Id)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(p => new ProductListViewModel.Row
            {
                Id = p.Id,
                Name = p.Name,
                ImageUrl = p.Images.OrderBy(i => i.SortOrder).Select(i => i.Url).FirstOrDefault(),
                CategoryName = p.Category.Name,
                Price = p.Price,
                Stock = p.Variants.Sum(v => (int?)v.StockQuantity) ?? 0,
                IsActive = p.IsActive,
                IsFeatured = p.IsFeatured,
                Badge = p.Badge,
            })
            .ToListAsync();

        return View(new ProductListViewModel
        {
            Products = rows,
            Page = page,
            TotalPages = totalPages,
            TotalCount = total,
            Query = q,
            CategoryId = categoryId,
            Categories = await Db.Categories.OrderBy(c => c.Name).ToListAsync(),
        });
    }

    [HttpGet("/admin/san-pham/them")]
    public async Task<IActionResult> Create()
    {
        return View("Edit", new ProductEditViewModel
        {
            IsActive = true,
            Categories = await Db.Categories.OrderBy(c => c.Type).ThenBy(c => c.SortOrder).ToListAsync(),
        });
    }

    [HttpGet("/admin/san-pham/{id:int}/sua")]
    public async Task<IActionResult> Edit(int id)
    {
        var p = await Db.Products
            .Include(x => x.Images)
            .Include(x => x.Variants)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (p is null) return NotFound();

        return View(new ProductEditViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Slug = p.Slug,
            Description = p.Description,
            Price = p.Price,
            CompareAtPrice = p.CompareAtPrice,
            Badge = p.Badge,
            IsFeatured = p.IsFeatured,
            IsActive = p.IsActive,
            SortOrder = p.SortOrder,
            CategoryId = p.CategoryId,
            MetaTitle = p.MetaTitle,
            MetaDescription = p.MetaDescription,
            ImageUrls = p.Images.OrderBy(i => i.SortOrder).Select(i => i.Url).ToList(),
            Variants = p.Variants.Select(v => new ProductEditViewModel.VariantRow
            {
                Id = v.Id, Name = v.Name, PriceModifier = v.PriceModifier, Sku = v.Sku, StockQuantity = v.StockQuantity,
            }).ToList(),
            Categories = await Db.Categories.OrderBy(c => c.Type).ThenBy(c => c.SortOrder).ToListAsync(),
        });
    }

    public class ProductForm
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? CompareAtPrice { get; set; }
        public ProductBadge Badge { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public int CategoryId { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }

        // Parallel arrays from the dynamic form rows.
        public List<string>? ImageUrls { get; set; }
        public List<int>? VariantIds { get; set; }
        public List<string>? VariantNames { get; set; }
        public List<decimal>? VariantPrices { get; set; }
        public List<string>? VariantSkus { get; set; }
        public List<int>? VariantStocks { get; set; }
    }

    [HttpPost("/admin/san-pham/luu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(ProductForm form)
    {
        if (string.IsNullOrWhiteSpace(form.Name))
        {
            Fail("Tên sản phẩm không được để trống.");
            return RedirectToAction(form.Id == 0 ? nameof(Create) : nameof(Edit), form.Id == 0 ? null : new { id = form.Id });
        }

        var slug = string.IsNullOrWhiteSpace(form.Slug) ? Slug.From(form.Name) : Slug.From(form.Slug);
        if (await Db.Products.AnyAsync(p => p.Slug == slug && p.Id != form.Id))
        {
            Fail($"Slug \"{slug}\" đã tồn tại.");
            return RedirectToAction(form.Id == 0 ? nameof(Create) : nameof(Edit), form.Id == 0 ? null : new { id = form.Id });
        }

        var product = form.Id == 0
            ? new Product { Name = "", Slug = "" }
            : await Db.Products.Include(p => p.Images).Include(p => p.Variants).FirstOrDefaultAsync(p => p.Id == form.Id);
        if (product is null) return NotFound();

        product.Name = form.Name.Trim();
        product.Slug = slug;
        product.Description = form.Description;
        product.Price = form.Price;
        product.CompareAtPrice = form.CompareAtPrice;
        product.Badge = form.Badge;
        product.IsFeatured = form.IsFeatured;
        product.IsActive = form.IsActive;
        product.SortOrder = form.SortOrder;
        product.CategoryId = form.CategoryId;
        product.MetaTitle = form.MetaTitle;
        product.MetaDescription = form.MetaDescription;

        if (form.Id == 0)
        {
            product.CreatedAt = DateTime.UtcNow;
            Db.Products.Add(product);
        }
        else
        {
            product.UpdatedAt = DateTime.UtcNow;
        }

        SyncImages(product, form.ImageUrls ?? []);
        SyncVariants(product, form);

        auth.Audit(form.Id == 0 ? "Thêm sản phẩm" : "Sửa sản phẩm", nameof(Product), form.Id == 0 ? null : form.Id, form.Name);
        await Db.SaveChangesAsync();
        Ok(form.Id == 0 ? "Đã thêm sản phẩm." : "Đã lưu sản phẩm.");
        return RedirectToAction(nameof(Edit), new { id = product.Id });
    }

    private static void SyncImages(Product product, List<string> urls)
    {
        // Replace the whole set — the form always posts the current, ordered list.
        product.Images.Clear();
        var order = 0;
        foreach (var url in urls.Where(u => !string.IsNullOrWhiteSpace(u)))
        {
            product.Images.Add(new ProductImage { Url = url, SortOrder = order++ });
        }
    }

    private void SyncVariants(Product product, ProductForm form)
    {
        var names = form.VariantNames ?? [];
        var prices = form.VariantPrices ?? [];
        var skus = form.VariantSkus ?? [];
        var stocks = form.VariantStocks ?? [];
        var ids = form.VariantIds ?? [];

        // Rows the form no longer contains are deletions.
        var keptIds = ids.Where(i => i > 0).ToHashSet();
        foreach (var existing in product.Variants.Where(v => !keptIds.Contains(v.Id)).ToList())
        {
            product.Variants.Remove(existing);
        }

        for (var i = 0; i < names.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(names[i])) continue;

            var id = i < ids.Count ? ids[i] : 0;
            var variant = id > 0 ? product.Variants.FirstOrDefault(v => v.Id == id) : null;
            if (variant is null)
            {
                variant = new ProductVariant { Name = "" };
                product.Variants.Add(variant);
            }
            variant.Name = names[i].Trim();
            variant.PriceModifier = i < prices.Count ? prices[i] : 0;
            variant.Sku = i < skus.Count && !string.IsNullOrWhiteSpace(skus[i]) ? skus[i].Trim() : null;
            variant.StockQuantity = i < stocks.Count ? stocks[i] : 0;
        }
    }

    [HttpPost("/admin/san-pham/{id:int}/an-hien")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var product = await Db.Products.FindAsync(id);
        if (product is null) return NotFound();
        product.IsActive = !product.IsActive;
        product.UpdatedAt = DateTime.UtcNow;
        auth.Audit(product.IsActive ? "Hiện sản phẩm" : "Ẩn sản phẩm", nameof(Product), id, product.Name);
        await Db.SaveChangesAsync();
        Ok(product.IsActive ? "Đã hiện sản phẩm." : "Đã ẩn sản phẩm.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/admin/san-pham/{id:int}/xoa")]
    [Authorize(Policy = AdminAuth.PolicyOwner)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await Db.Products.FindAsync(id);
        if (product is null) return NotFound();

        // OrderItem keeps a name/price snapshot (no FK), so removing a product never breaks past
        // orders. Images and variants cascade.
        Db.Products.Remove(product);
        auth.Audit("Xóa sản phẩm", nameof(Product), id, product.Name);
        await Db.SaveChangesAsync();
        Ok("Đã xóa sản phẩm.");
        return RedirectToAction(nameof(Index));
    }
}
