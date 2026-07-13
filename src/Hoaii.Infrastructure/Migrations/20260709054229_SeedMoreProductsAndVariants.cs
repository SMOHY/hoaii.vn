using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hoaii.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedMoreProductsAndVariants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProductVariants",
                columns: new[] { "Id", "Name", "PriceModifier", "ProductId", "Sku", "StockQuantity" },
                values: new object[,]
                {
                    { 1, "Hộp 4 túi / màu vàng", 0m, 1, null, 50 },
                    { 2, "Hộp 4 túi / màu đỏ", 0m, 1, null, 30 },
                    { 3, "Hộp 8 túi / màu vàng", 80000m, 1, null, 20 },
                    { 4, "Hộp gỗ nhỏ", 0m, 3, null, 15 },
                    { 5, "Hộp gỗ lớn", 120000m, 3, null, 10 }
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Badge", "CategoryId", "IsFeatured", "Name", "Price", "Slug" },
                values: new object[] { 0, 1, false, "Trà sen Tây Hồ hộp gỗ", 410000m, "tra-sen-tay-ho-hop-go" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Badge", "CategoryId", "Name", "Price", "Slug" },
                values: new object[] { 1, 1, "Trà shan tuyết cổ thụ", 550000m, "tra-shan-tuyet-co-thu" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Badge", "CategoryId", "CompareAtPrice", "Description", "IsFeatured", "Name", "Price", "Slug" },
                values: new object[,]
                {
                    { 5, 3, 1, null, null, false, "Trà lài ướp hương", 190000m, "tra-lai-uop-huong" },
                    { 6, 0, 1, null, null, false, "Trà đen kỵ sữa", 220000m, "tra-den-ky-sua" },
                    { 7, 2, 1, 210000m, null, false, "Trà thảo mộc detox", 175000m, "tra-thao-moc-detox" },
                    { 8, 0, 1, null, null, false, "Trà atiso Đà Lạt", 205000m, "tra-atiso-da-lat" },
                    { 9, 0, 1, null, null, false, "Trà oolong túi lọc cao cấp", 260000m, "tra-oolong-tui-loc-cao-cap" },
                    { 10, 0, 1, null, null, false, "Trà bạc hà hộp thiếc", 165000m, "tra-bac-ha-hop-thiec" },
                    { 20, 1, 5, null, null, true, "Hộp quà Tết Xuân Phú Quý", 890000m, "hop-qua-tet-xuan-phu-quy" },
                    { 21, 0, 5, null, null, false, "Hộp quà Tết An Khang", 650000m, "hop-qua-tet-an-khang" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Badge", "CategoryId", "IsFeatured", "Name", "Price", "Slug" },
                values: new object[] { 1, 5, true, "Hộp quà Tết Xuân Phú Quý", 890000m, "hop-qua-tet-xuan-phu-quy" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Badge", "CategoryId", "Name", "Price", "Slug" },
                values: new object[] { 0, 5, "Hộp quà Tết An Khang", 650000m, "hop-qua-tet-an-khang" });
        }
    }
}
