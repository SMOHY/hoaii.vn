using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hoaii.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBlogPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlogPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Excerpt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPosts", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "BlogPosts",
                columns: new[] { "Id", "Category", "Excerpt", "ImageUrl", "IsFeatured", "PublishedAt", "Slug", "Title" },
                values: new object[,]
                {
                    { 1, "Đời sống", "Chọn quà tặng sao cho vừa ý nghĩa vừa tinh tế luôn là điều khiến nhiều người trăn trở. Cùng HOÀI khám phá những gợi ý quà tặng phù hợp với từng đối tượng và dịp lễ trong năm.", null, true, new DateTime(2026, 7, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "goi-y-chon-qua-tang-nguoi-than", "Gợi ý chọn quà tặng cho người thân yêu" },
                    { 2, "Văn hóa", "Khám phá quy trình ướp trà sen truyền thống của người Hà Nội, một nét đẹp văn hóa được gìn giữ qua nhiều thế hệ.", null, false, new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "tra-sen-tay-ho-tinh-hoa-tra-viet", "Trà sen Tây Hồ — tinh hoa trà Việt trăm năm" },
                    { 3, "Xu hướng", "Tổng hợp những mẫu hộp quà Tết bán chạy nhất mùa Tết 2026 tại HOÀI, phù hợp biếu tặng đối tác và người thân.", null, false, new DateTime(2026, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "5-mau-hop-qua-tet-yeu-thich-2026", "5 mẫu hộp quà Tết được yêu thích nhất 2026" },
                    { 4, "Đời sống", "Furoshiki không chỉ là cách gói quà mà còn là một nghệ thuật thể hiện sự trân trọng dành cho người nhận.", null, false, new DateTime(2026, 5, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "nghe-thuat-goi-qua-furoshiki", "Nghệ thuật gói quà kiểu Nhật Furoshiki" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_Slug",
                table: "BlogPosts",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogPosts");
        }
    }
}
