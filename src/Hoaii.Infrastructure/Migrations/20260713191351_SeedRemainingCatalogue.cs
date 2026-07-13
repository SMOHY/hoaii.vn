using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hoaii.Infrastructure.Migrations
{
    /// <summary>
    /// Khăn, Tượng gốm, Rượu, Quà trung thu and Quà tặng theo dịp are all linked from the nav but
    /// had no products, so those category pages rendered "Chưa có sản phẩm nào". The ten tea
    /// products had no imagery either, leaving the grid and search results full of empty tiles.
    ///
    /// Figma's own pages for these categories are still template placeholders (the same photo
    /// repeated), so the seed reuses the real HOÀI product photography rather than inventing more.
    /// </summary>
    public partial class SeedRemainingCatalogue : Migration
    {
        private const int FirstProductId = 40;
        private const int FirstImageId = 100;

        private static readonly string[] Photos =
        [
            "/images/products/thien-dieu-lac-hong.jpg",
            "/images/products/tinh-hoa-bac-bo.jpg",
            "/images/products/phung-hoa-trinh-tuong.jpg",
            "/images/products/ngu-qua.jpg",
            "/images/products/viet-nam-hao-ca.jpg",
            "/images/products/viet-nam-hoa-thi.jpg",
            "/images/placeholders/featured-2.jpg",
            "/images/placeholders/featured-3.jpg",
            "/images/placeholders/featured-4.jpg",
            "/images/placeholders/featured-5.jpg",
        ];

        // (CategoryId, Name, Slug, Price)
        private static readonly (int Cat, string Name, string Slug, decimal Price)[] NewProducts =
        [
            // Khăn (2)
            (2, "Khăn lụa Vân Phong", "khan-lua-van-phong", 450000m),
            (2, "Khăn lụa Hà Đông", "khan-lua-ha-dong", 520000m),
            (2, "Khăn tơ tằm Nha Xá", "khan-to-tam-nha-xa", 680000m),
            (2, "Khăn choàng thổ cẩm", "khan-choang-tho-cam", 390000m),
            (2, "Khăn lụa hoa văn Đông Sơn", "khan-lua-hoa-van-dong-son", 750000m),
            (2, "Khăn lụa thêu tay", "khan-lua-theu-tay", 890000m),

            // Tượng gốm (3)
            (3, "Tượng gốm Bát Tràng", "tuong-gom-bat-trang", 620000m),
            (3, "Tượng ngựa gốm men lam", "tuong-ngua-gom-men-lam", 850000m),
            (3, "Tượng nghê gốm cổ", "tuong-nghe-gom-co", 980000m),
            (3, "Bình gốm hoa nâu", "binh-gom-hoa-nau", 720000m),
            (3, "Đĩa gốm khắc hoa văn", "dia-gom-khac-hoa-van", 480000m),
            (3, "Tượng gốm Tứ Linh", "tuong-gom-tu-linh", 1250000m),

            // Rượu (4)
            (4, "Rượu Em Mơ", "ruou-em-mo", 890000m),
            (4, "Rượu nếp cái hoa vàng", "ruou-nep-cai-hoa-vang", 550000m),
            (4, "Rượu mơ Yên Tử", "ruou-mo-yen-tu", 480000m),
            (4, "Rượu sim Phú Quốc", "ruou-sim-phu-quoc", 420000m),
            (4, "Rượu táo mèo Tây Bắc", "ruou-tao-meo-tay-bac", 390000m),
            (4, "Rượu vang Đà Lạt", "ruou-vang-da-lat", 650000m),

            // Quà trung thu (6)
            (6, "Hộp bánh Tinh Hoa Bắc Bộ (4 bánh)", "hop-banh-tinh-hoa-bac-bo-4", 595000m),
            (6, "Hộp bánh Tinh Hoa Bắc Bộ (6 bánh)", "hop-banh-tinh-hoa-bac-bo-6", 795000m),
            (6, "Hộp bánh Việt Nam Hoa Thị", "hop-banh-viet-nam-hoa-thi", 899000m),
            (6, "Hộp bánh Thiên Điểu Lạc Hồng", "hop-banh-thien-dieu-lac-hong", 899000m),
            (6, "Hộp bánh Ngũ Quả", "hop-banh-ngu-qua", 750000m),
            (6, "Hộp bánh Phụng Hoa Trình Tường", "hop-banh-phung-hoa-trinh-tuong", 850000m),

            // Quà tặng theo dịp (12)
            (12, "Set quà Tri Ân", "set-qua-tri-an", 990000m),
            (12, "Set quà Khai Trương", "set-qua-khai-truong", 1250000m),
            (12, "Set quà Sinh Nhật", "set-qua-sinh-nhat", 680000m),
            (12, "Set quà Cưới Hỏi", "set-qua-cuoi-hoi", 1450000m),
            (12, "Set quà Doanh Nghiệp", "set-qua-doanh-nghiep", 1890000m),
            (12, "Set quà Tân Gia", "set-qua-tan-gia", 890000m),
        ];

        // Tea products already exist (ids 1-10) but have no imagery.
        private static readonly int[] TeaProductIds = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var imageId = FirstImageId;

            for (var i = 0; i < NewProducts.Length; i++)
            {
                var p = NewProducts[i];
                var productId = FirstProductId + i;

                migrationBuilder.InsertData(
                    table: "Products",
                    columns: new[] { "Id", "Name", "Slug", "Price", "CompareAtPrice", "Description", "Badge", "IsFeatured", "CategoryId" },
                    values: new object[] { productId, p.Name, p.Slug, p.Price, null, null, 0, false, p.Cat });

                migrationBuilder.InsertData(
                    table: "ProductImages",
                    columns: new[] { "Id", "ProductId", "Url", "SortOrder" },
                    values: new object[] { imageId++, productId, Photos[i % Photos.Length], 0 });
            }

            for (var i = 0; i < TeaProductIds.Length; i++)
            {
                migrationBuilder.InsertData(
                    table: "ProductImages",
                    columns: new[] { "Id", "ProductId", "Url", "SortOrder" },
                    values: new object[] { imageId++, TeaProductIds[i], Photos[i % Photos.Length], 0 });
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var total = NewProducts.Length + TeaProductIds.Length;
            for (var i = 0; i < total; i++)
            {
                migrationBuilder.DeleteData(table: "ProductImages", keyColumn: "Id", keyValue: FirstImageId + i);
            }

            for (var i = 0; i < NewProducts.Length; i++)
            {
                migrationBuilder.DeleteData(table: "Products", keyColumn: "Id", keyValue: FirstProductId + i);
            }
        }
    }
}
