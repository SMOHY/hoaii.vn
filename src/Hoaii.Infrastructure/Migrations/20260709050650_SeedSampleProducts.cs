using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hoaii.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedSampleProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Badge", "CategoryId", "CompareAtPrice", "Description", "IsFeatured", "Name", "Price", "Slug" },
                values: new object[,]
                {
                    { 1, 1, 1, null, null, false, "Trà sen vàng", 250000m, "tra-sen-vang" },
                    { 2, 2, 1, 380000m, null, false, "Trà ô long thượng hạng", 320000m, "tra-o-long-thuong-hang" },
                    { 3, 1, 5, null, null, true, "Hộp quà Tết Xuân Phú Quý", 890000m, "hop-qua-tet-xuan-phu-quy" },
                    { 4, 0, 5, null, null, false, "Hộp quà Tết An Khang", 650000m, "hop-qua-tet-an-khang" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
