using Hoaii.Infrastructure;
using Hoaii.Web.Models.Category;
using Hoaii.Web.Models.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Controllers;

public class ProductController(HoaiiDbContext db) : Controller
{
    public async Task<IActionResult> Details(string slug)
    {
        // Hidden products are gone from the storefront entirely — the admin's "Đang bán" switch
        // used to change nothing out here, so a hidden product stayed viewable and buyable.
        var product = await db.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive);

        if (product is null)
        {
            return NotFound();
        }

        var related = await db.Products
            .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id && p.IsActive)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .Take(4)
            .ToListAsync();

        // Only real shots — padding the strip out to five left grey squares on the page.
        var galleryImages = product.Images
            .OrderBy(i => i.SortOrder)
            .Select(i => (string?)i.Url)
            .ToList();

        CollectionSectionViewModel? collection = null;
        if (product.IsFeatured)
        {
            // Figma always draws three tiles here (node 826:20798). Featured siblings come
            // first; the rest of the category fills the row so it never renders half-empty.
            var collectionItems = await db.Products
                .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id && p.IsActive)
                .OrderByDescending(p => p.IsFeatured)
                .ThenBy(p => p.Id)
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .Take(3)
                .ToListAsync();

            if (collectionItems.Count > 0)
            {
                collection = new CollectionSectionViewModel
                {
                    Eyebrow = "Khám phá bộ sưu tập",
                    Title = $"BỘ SƯU TẬP {product.Category.Name.ToUpperInvariant()}",
                    Items = collectionItems.Select(p => ProductCardMapper.Map(p, "collection")).ToList(),
                };
            }
        }

        var model = new ProductDetailsViewModel
        {
            ProductId = product.Id,
            Slug = product.Slug,
            Name = product.Name,
            Price = product.Price,
            BreadcrumbLabel = $"Trang chủ/{product.Category.Name}",
            GalleryImages = galleryImages,
            BoxOptions = product.Variants
                .Select(v => new BoxOptionViewModel { Id = v.Id, Label = v.Name })
                .ToList(),
            Ingredients = product.Description
                ?? "Thông tin thành phần sẽ được cập nhật chi tiết theo từng sản phẩm.",
            StoryTitle = product.StoryTitle is { Length: > 0 } st ? st : "Câu chuyện sản phẩm",
            StoryBody = product.StoryBody is { Length: > 0 } sb ? sb
                : $"{product.Name} được HOÀI chế tác tỉ mỉ, gói ghém tinh thần văn hóa Việt trong từng chi tiết — từ nguyên liệu chọn lọc đến bao bì thủ công, mang đến một món quà trọn vẹn ý nghĩa.",
            StoryImageUrl = product.StoryImageUrl is { Length: > 0 } si ? si : "/images/pdp/story.jpg",
            FeatureTitle = product.FeatureTitle is { Length: > 0 } ft ? ft : "Đặc điểm",
            FeatureBody = product.FeatureBody is { Length: > 0 } fb ? fb
                : "KÍCH THƯỚC:\nHộp cứng: 48x15.7x6cm\nHộp con: 9.5x9.5x5cm\nQuai xách: 38x15.7x6.2cm\nTúi đựng: 49x17x7cm",
            FeatureImageUrl = product.FeatureImageUrl is { Length: > 0 } fi ? fi : "/images/pdp/feature.jpg",
            Collection = collection,
            RelatedProducts = related.Select(p => ProductCardMapper.Map(p, "related")).ToList(),
        };

        return View(model);
    }
}
