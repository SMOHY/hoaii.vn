using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hoaii.Infrastructure.Migrations
{
    /// <summary>
    /// The Quà tết category held two generic sample rows with no imagery, so /danh-muc/qua-tet
    /// rendered two empty tiles. Figma shows the six real collections at 899.000đ
    /// (node 1519:34031). Ids 20 and 21 are updated in place rather than deleted, since orders
    /// and cart lines may already reference them.
    /// </summary>
    public partial class SeedTetProductsFromFigma : Migration
    {
        private const int TetCategoryId = 5;
        private const decimal Price = 899000m;

        private static readonly (int Id, string Name, string Slug, int Badge, bool Featured)[] Products =
        [
            (20, "Thiên điểu lạc hồng",   "thien-dieu-lac-hong",   1, true),
            (21, "Tinh hoa bắc bộ",       "tinh-hoa-bac-bo",       0, true),
            (30, "Phụng hoa trình tường", "phung-hoa-trinh-tuong", 0, false),
            (31, "Ngũ quả",               "ngu-qua",               0, false),
            (32, "Việt Nam Hạo Ca",       "viet-nam-hao-ca",       0, false),
            (33, "Việt Nam Hoa Thị",      "viet-nam-hoa-thi",      1, false),
        ];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Re-point the two existing sample rows at the real collections.
            foreach (var p in Products[..2])
            {
                migrationBuilder.UpdateData(
                    table: "Products",
                    keyColumn: "Id",
                    keyValue: p.Id,
                    columns: new[] { "Name", "Slug", "Price", "Badge", "IsFeatured", "CategoryId" },
                    values: new object[] { p.Name, p.Slug, Price, p.Badge, p.Featured, TetCategoryId });
            }

            foreach (var p in Products[2..])
            {
                migrationBuilder.InsertData(
                    table: "Products",
                    columns: new[] { "Id", "Name", "Slug", "Price", "CompareAtPrice", "Description", "Badge", "IsFeatured", "CategoryId" },
                    values: new object[] { p.Id, p.Name, p.Slug, Price, null, null, p.Badge, p.Featured, TetCategoryId });
            }

            var imageId = 1;
            foreach (var p in Products)
            {
                migrationBuilder.InsertData(
                    table: "ProductImages",
                    columns: new[] { "Id", "ProductId", "Url", "SortOrder" },
                    values: new object[] { imageId++, p.Id, $"/images/products/{p.Slug}.jpg", 0 });
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (var id = 1; id <= Products.Length; id++)
            {
                migrationBuilder.DeleteData(table: "ProductImages", keyColumn: "Id", keyValue: id);
            }

            foreach (var p in Products[2..])
            {
                migrationBuilder.DeleteData(table: "Products", keyColumn: "Id", keyValue: p.Id);
            }

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Name", "Slug", "Price", "Badge", "IsFeatured" },
                values: new object[] { "Hộp quà Tết Xuân Phú Quý", "hop-qua-tet-xuan-phu-quy", 890000m, 1, true });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Name", "Slug", "Price", "Badge", "IsFeatured" },
                values: new object[] { "Hộp quà Tết An Khang", "hop-qua-tet-an-khang", 650000m, 0, false });
        }
    }
}
