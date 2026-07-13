using Hoaii.Infrastructure;
using Hoaii.Web.Models.Category;
using Hoaii.Web.Models.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Controllers;

public class ProductController(HoaiiDbContext db) : Controller
{
    // Demo color palette used until real per-product color variants are modeled.
    private static readonly ColorOptionViewModel[] DemoColors =
    [
        new() { Name = "đỏ", Hex = "#870000" },
        new() { Name = "vàng", Hex = "#AA8656" },
        new() { Name = "xanh", Hex = "#00488C" },
    ];

    public async Task<IActionResult> Details(string slug)
    {
        var product = await db.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Slug == slug);

        if (product is null)
        {
            return NotFound();
        }

        var related = await db.Products
            .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .Take(4)
            .ToListAsync();

        var galleryImages = product.Images
            .OrderBy(i => i.SortOrder)
            .Select(i => (string?)i.Url)
            .ToList();
        while (galleryImages.Count < 5)
        {
            galleryImages.Add(null); // render as placeholder thumbnail
        }

        CollectionSectionViewModel? collection = null;
        if (product.IsFeatured)
        {
            var collectionItems = await db.Products
                .Where(p => p.CategoryId == product.CategoryId && p.IsFeatured && p.Id != product.Id)
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
            ColorOptions = DemoColors,
            BoxOptions = product.Variants
                .Select(v => new BoxOptionViewModel { Id = v.Id, Label = v.Name })
                .ToList(),
            Ingredients = product.Description
                ?? "Thông tin thành phần sẽ được cập nhật chi tiết theo từng sản phẩm.",
            StoryTitle = "Câu chuyện sản phẩm",
            StoryBody = $"{product.Name} được HOÀI chế tác tỉ mỉ, gói ghém tinh thần văn hóa Việt trong từng chi tiết — từ nguyên liệu chọn lọc đến bao bì thủ công, mang đến một món quà trọn vẹn ý nghĩa.",
            FeatureTitle = "Đặc điểm",
            FeatureBody = "Thiết kế tinh giản, chất liệu bền vững và quy trình đóng gói an toàn giúp sản phẩm luôn giữ được trọn vẹn hương vị và giá trị khi đến tay người nhận.",
            Collection = collection,
            RelatedProducts = related.Select(p => ProductCardMapper.Map(p, "related")).ToList(),
        };

        return View(model);
    }
}
