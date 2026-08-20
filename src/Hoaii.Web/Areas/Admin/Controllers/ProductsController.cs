using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Areas.Admin.Models;
using Hoaii.Web.Services;
using Hoaii.Web.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hoaii.Web.Areas.Admin.Filters;

namespace Hoaii.Web.Areas.Admin.Controllers;

public class ProductsController(HoaiiDbContext db, AdminAuthService auth, MediaService media) : BaseAdminController(db)
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
            Collections = await Db.Collections.OrderBy(c => c.SortOrder).ToListAsync(),
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

        var relatedProducts = await Db.RelatedProducts
            .Where(r => r.ProductId == id)
            .OrderBy(r => r.SortOrder)
            .Select(r => new ProductEditViewModel.RelatedProductRow { Id = r.RelatedProductId, Name = r.RelatedTo!.Name })
            .ToListAsync();

        return View(new ProductEditViewModel
        {
            RelatedProducts = relatedProducts,
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
            CollectionId = p.CollectionId,
            MetaTitle = p.MetaTitle,
            MetaDescription = p.MetaDescription,
            StoryTitle = p.StoryTitle,
            StoryBody = p.StoryBody,
            StoryImageUrl = p.StoryImageUrl,
            StoryImageUrlMobile = p.StoryImageUrlMobile,
            StoryImageFocal = p.StoryImageFocal,
            FeatureTitle = p.FeatureTitle,
            FeatureBody = p.FeatureBody,
            FeatureImageUrl = p.FeatureImageUrl,
            FeatureImageUrlMobile = p.FeatureImageUrlMobile,
            FeatureImageFocal = p.FeatureImageFocal,
            VideoFileUrl = p.VideoFileUrl,
            VideoEmbedUrl = p.VideoEmbedUrl,
            VideoPosterUrl = p.VideoPosterUrl,
            ImageUrls = p.Images.OrderBy(i => i.SortOrder).Select(i => i.Url).ToList(),
            Variants = p.Variants.Select(v => new ProductEditViewModel.VariantRow
            {
                Id = v.Id, Name = v.Name, PriceModifier = v.PriceModifier, Sku = v.Sku, StockQuantity = v.StockQuantity,
            }).ToList(),
            Categories = await Db.Categories.OrderBy(c => c.Type).ThenBy(c => c.SortOrder).ToListAsync(),
            Collections = await Db.Collections.OrderBy(c => c.SortOrder).ToListAsync(),
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
        public int? CollectionId { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }

        // Product detail page copy.
        public string? StoryTitle { get; set; }
        public string? StoryBody { get; set; }
        public string? StoryImageUrl { get; set; }
        public string? StoryImageUrlMobile { get; set; }
        public string? StoryImageFocal { get; set; }
        public string? FeatureTitle { get; set; }
        public string? FeatureBody { get; set; }
        public string? FeatureImageUrl { get; set; }
        public string? FeatureImageUrlMobile { get; set; }
        public string? FeatureImageFocal { get; set; }

        /// <summary>Tệp video admin chọn từ máy. Rỗng nghĩa là giữ nguyên video đang có.</summary>
        public IFormFile? VideoFile { get; set; }

        public bool RemoveVideoFile { get; set; }

        public string? VideoEmbedUrl { get; set; }

        public string? VideoPosterUrl { get; set; }

        // Parallel arrays from the dynamic form rows.
        public List<string>? ImageUrls { get; set; }
        public List<int>? VariantIds { get; set; }
        public List<string>? VariantNames { get; set; }
        public List<decimal>? VariantPrices { get; set; }
        public List<string>? VariantSkus { get; set; }
        public List<int>? VariantStocks { get; set; }

        public List<int>? RelatedProductIds { get; set; }
    }

    /// <summary>
    /// Trả người dùng về đúng cái form họ vừa gõ, kèm lời báo lỗi.
    ///
    /// Trước đây mọi nhánh lỗi đều <c>Fail(...) + RedirectToAction(Edit)</c>. Redirect nạp lại
    /// trang từ CSDL, nên toàn bộ chữ vừa gõ — tên, mô tả, câu chuyện, đặc điểm, thứ tự ảnh,
    /// biến thể — biến mất sạch, chỉ còn một dòng báo lỗi. Đo được: đổi tên sản phẩm rồi chọn
    /// nhầm một tệp không phải video, tên quay về giá trị cũ. Người dùng mất công gõ lại từ đầu
    /// mà không hiểu vì sao.
    ///
    /// Ở đây dựng thẳng view-model từ <paramref name="form"/> nên chữ còn nguyên; chỉ những thứ
    /// không nằm trong form (danh sách danh mục, bộ sưu tập) mới lấy lại từ CSDL.
    /// </summary>
    private async Task<IActionResult> QuayLaiForm(ProductForm form, string loi)
    {
        Fail(loi);

        var tenLienQuan = form.RelatedProductIds is { Count: > 0 }
            ? await Db.Products.Where(p => form.RelatedProductIds.Contains(p.Id))
                .Select(p => new { p.Id, p.Name }).ToListAsync()
            : [];

        var bienThe = new List<ProductEditViewModel.VariantRow>();
        var ten = form.VariantNames ?? [];
        for (var i = 0; i < ten.Count; i++)
        {
            bienThe.Add(new ProductEditViewModel.VariantRow
            {
                Id = form.VariantIds is { } vid && i < vid.Count ? vid[i] : 0,
                Name = ten[i],
                PriceModifier = form.VariantPrices is { } vp && i < vp.Count ? vp[i] : 0,
                Sku = form.VariantSkus is { } vs && i < vs.Count ? vs[i] : null,
                StockQuantity = form.VariantStocks is { } vst && i < vst.Count ? vst[i] : 0,
            });
        }

        return View("Edit", new ProductEditViewModel
        {
            Id = form.Id,
            Name = form.Name ?? "",
            Slug = form.Slug,
            Description = form.Description,
            Price = form.Price,
            CompareAtPrice = form.CompareAtPrice,
            Badge = form.Badge,
            IsFeatured = form.IsFeatured,
            IsActive = form.IsActive,
            SortOrder = form.SortOrder,
            CategoryId = form.CategoryId,
            CollectionId = form.CollectionId,
            MetaTitle = form.MetaTitle,
            MetaDescription = form.MetaDescription,
            StoryTitle = form.StoryTitle,
            StoryBody = form.StoryBody,
            StoryImageUrl = form.StoryImageUrl,
            StoryImageUrlMobile = form.StoryImageUrlMobile,
            StoryImageFocal = form.StoryImageFocal,
            FeatureTitle = form.FeatureTitle,
            FeatureBody = form.FeatureBody,
            FeatureImageUrl = form.FeatureImageUrl,
            FeatureImageUrlMobile = form.FeatureImageUrlMobile,
            FeatureImageFocal = form.FeatureImageFocal,
            // Tệp video vừa chọn không giữ lại được qua một vòng request — trình duyệt không cho
            // điền sẵn ô chọn tệp. Đường dẫn video đang lưu thì lấy lại từ CSDL để ô không trống.
            VideoFileUrl = form.Id == 0 ? null : await Db.Products.Where(p => p.Id == form.Id)
                .Select(p => p.VideoFileUrl).FirstOrDefaultAsync(),
            VideoEmbedUrl = form.VideoEmbedUrl,
            VideoPosterUrl = form.VideoPosterUrl,
            ImageUrls = form.ImageUrls ?? [],
            Variants = bienThe,
            RelatedProducts = (form.RelatedProductIds ?? [])
                .Select(id => new ProductEditViewModel.RelatedProductRow
                {
                    Id = id,
                    Name = tenLienQuan.FirstOrDefault(x => x.Id == id)?.Name ?? "",
                })
                .ToList(),
            Categories = await Db.Categories.OrderBy(c => c.Type).ThenBy(c => c.SortOrder).ToListAsync(),
            Collections = await Db.Collections.OrderBy(c => c.SortOrder).ToListAsync(),
        });
    }

    [HttpPost("/admin/san-pham/luu")]
    [GioiHanTep(MediaService.MaxVideoBytes, MediaService.MaxVideoLabel)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(ProductForm form)
    {
        if (string.IsNullOrWhiteSpace(form.Name))
        {
            return await QuayLaiForm(form, "Tên sản phẩm không được để trống.");
        }

        var slug = string.IsNullOrWhiteSpace(form.Slug) ? Slug.From(form.Name) : Slug.From(form.Slug);
        if (await Db.Products.AnyAsync(p => p.Slug == slug && p.Id != form.Id))
        {
            return await QuayLaiForm(form, $"Slug \"{slug}\" đã tồn tại.");
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
        product.CollectionId = form.CollectionId;
        product.MetaTitle = form.MetaTitle;
        product.MetaDescription = form.MetaDescription;
        product.StoryTitle = Clean(form.StoryTitle);
        product.StoryBody = Clean(form.StoryBody);
        product.StoryImageUrl = Clean(form.StoryImageUrl);
        product.StoryImageUrlMobile = Clean(form.StoryImageUrlMobile);
        product.StoryImageFocal = Focal(form.StoryImageFocal);
        product.FeatureTitle = Clean(form.FeatureTitle);
        product.FeatureBody = Clean(form.FeatureBody);
        product.FeatureImageUrl = Clean(form.FeatureImageUrl);
        product.FeatureImageUrlMobile = Clean(form.FeatureImageUrlMobile);
        product.FeatureImageFocal = Focal(form.FeatureImageFocal);

        // Video: tệp tải lên thắng link nhúng khi có cả hai — trang sản phẩm chỉ hiện được một.
        // Đổi video là việc hiếm và nặng, nên chỉ động vào khi admin thực sự chọn tệp mới hoặc
        // tick xoá; không bao giờ ghi rỗng đè lên video đang có chỉ vì ô tệp để trống.
        if (form.RemoveVideoFile)
        {
            product.VideoFileUrl = null;
        }
        if (form.VideoFile is { Length: > 0 })
        {
            var upload = await media.UploadVideoAsync(form.VideoFile);
            if (!upload.Ok)
            {
                return await QuayLaiForm(form, upload.Error ?? "Không tải được video.");
            }
            product.VideoFileUrl = upload.Url;
        }
        // Ô để trống nghĩa là bỏ video; gõ sai KHÔNG có nghĩa là bỏ. Gộp hai thứ này lại chính
        // là thứ đã xoá mất video của sản phẩm mà vẫn báo "Đã lưu".
        switch (VideoLink.Doc(form.VideoEmbedUrl, out var linkVideo))
        {
            case VideoLink.KetQua.BoTrong:
                product.VideoEmbedUrl = null;
                break;
            case VideoLink.KetQua.HopLe:
                product.VideoEmbedUrl = linkVideo;
                break;
            default:
                return await QuayLaiForm(form,
                    "Không nhận ra link video. Chỉ nhận link YouTube hoặc Vimeo — hãy dán nguyên đường dẫn " +
                    "trên thanh địa chỉ. Link cũ vẫn được giữ nguyên.");
        }
        product.VideoPosterUrl = string.IsNullOrWhiteSpace(form.VideoPosterUrl) ? null : form.VideoPosterUrl.Trim();

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
        SyncRelatedProducts(product, form.RelatedProductIds ?? []);

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

    /// <summary>Replaces the whole pick list, same "form always posts the current, ordered list"
    /// rule as SyncImages. Uses the Product navigation rather than product.Id so this still works
    /// when product is a brand-new, not-yet-saved row (Id is 0 until SaveChangesAsync — EF's
    /// relationship fixup resolves the real FK from the tracked Product instead).</summary>
    private void SyncRelatedProducts(Product product, List<int> relatedIds)
    {
        var ids = relatedIds.Where(id => id > 0 && id != product.Id).Distinct().Take(4).ToList();

        if (product.Id != 0)
        {
            var existing = Db.RelatedProducts.Where(r => r.ProductId == product.Id);
            Db.RelatedProducts.RemoveRange(existing);
        }

        for (var i = 0; i < ids.Count; i++)
        {
            Db.RelatedProducts.Add(new RelatedProduct { Product = product, RelatedProductId = ids[i], SortOrder = i });
        }
    }

    [HttpGet("/admin/san-pham/tim-kiem")]
    public async Task<IActionResult> SearchProducts(string? q, int? excludeId)
    {
        var query = Db.Products.Include(p => p.Images).Include(p => p.Category).Where(p => p.IsActive);
        if (excludeId is > 0)
        {
            query = query.Where(p => p.Id != excludeId);
        }
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

    /// <summary>Ẩn/hiện nhiều sản phẩm một lượt. Đặt hẳn trạng thái thay vì đảo từng cái: khi
    /// chọn cả nhóm đang lẫn ẩn và hiện, "đảo" cho ra kết quả không ai đoán được.</summary>
    [HttpPost("/admin/san-pham/an-hien-nhieu")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkToggleActive(int[] ids, bool active, string? returnUrl)
    {
        if (ids is null || ids.Length == 0)
        {
            Fail("Chưa chọn sản phẩm nào.");
            return Redirect(SafeReturn(returnUrl));
        }

        var products = await Db.Products.Where(p => ids.Contains(p.Id)).ToListAsync();
        var changed = 0;
        foreach (var product in products)
        {
            if (product.IsActive == active) continue;
            product.IsActive = active;
            product.UpdatedAt = DateTime.UtcNow;
            changed++;
        }

        if (changed > 0)
        {
            auth.Audit(active ? "Hiện sản phẩm hàng loạt" : "Ẩn sản phẩm hàng loạt", nameof(Product), null,
                $"{changed} sản phẩm");
            await Db.SaveChangesAsync();
        }

        Ok(changed == 0
            ? "Các sản phẩm đã chọn vốn đã ở trạng thái đó."
            : $"Đã {(active ? "hiện" : "ẩn")} {changed} sản phẩm.");
        return Redirect(SafeReturn(returnUrl));
    }

    /// <summary>Chỉ nhận đường dẫn nội bộ, để tham số trên URL không đẩy admin ra site khác.</summary>
    private static string SafeReturn(string? returnUrl) =>
        !string.IsNullOrWhiteSpace(returnUrl) && returnUrl.StartsWith('/') && !returnUrl.StartsWith("//")
            ? returnUrl
            : "/admin/san-pham";

    [HttpPost("/admin/san-pham/{id:int}/xoa")]
    [Authorize(Policy = AdminAuth.PolicyOwner)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await Db.Products.FindAsync(id);
        if (product is null) return NotFound();

        // OrderItem keeps a name/price snapshot (no FK), so removing a product never breaks past
        // orders. Images, variants, and this product's own related-picks (ProductId side) cascade;
        // rows where it's picked as someone ELSE's related product (RelatedProductId side) are
        // Restrict — see HoaiiDbContext — so they need removing by hand or the delete would fail.
        Db.RelatedProducts.RemoveRange(Db.RelatedProducts.Where(r => r.RelatedProductId == id));
        Db.Products.Remove(product);
        auth.Audit("Xóa sản phẩm", nameof(Product), id, product.Name);
        await Db.SaveChangesAsync();
        Ok("Đã xóa sản phẩm.");
        return RedirectToAction(nameof(Index));
    }
}
