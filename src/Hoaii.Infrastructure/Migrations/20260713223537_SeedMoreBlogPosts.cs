using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hoaii.Infrastructure.Migrations
{
    /// <summary>
    /// Figma's blog index shows one featured post plus a 3x2 grid (node 1154:36558); the
    /// database only had three posts behind the feature, so the second row was missing.
    /// </summary>
    public partial class SeedMoreBlogPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "BlogPosts",
                columns: new[] { "Id", "Title", "Slug", "Category", "Excerpt", "ImageUrl", "IsFeatured", "PublishedAt" },
                values: new object[,]
                {
                    {
                        20,
                        "Chọn quà tặng doanh nghiệp cuối năm sao cho tinh tế",
                        "qua-tang-doanh-nghiep-cuoi-nam",
                        "Xu hướng",
                        "Quà tặng cuối năm không chỉ là lời cảm ơn mà còn là cách doanh nghiệp kể câu chuyện thương hiệu của mình.",
                        "/images/placeholders/blog-2.jpg",
                        false,
                        new DateTime(2026, 5, 14, 0, 0, 0, DateTimeKind.Utc)
                    },
                    {
                        21,
                        "Gốm Bát Tràng và hành trình bảy thế kỷ",
                        "gom-bat-trang-bay-the-ky",
                        "Văn hóa",
                        "Từ đất sét bên bờ sông Hồng đến những vật phẩm được gìn giữ trong tủ kính, gốm Bát Tràng mang theo cả một vùng ký ức.",
                        "/images/placeholders/blog-3.jpg",
                        false,
                        new DateTime(2026, 4, 28, 0, 0, 0, DateTimeKind.Utc)
                    },
                    {
                        22,
                        "Bao bì quà tặng: khi lớp vỏ cũng là một phần món quà",
                        "bao-bi-qua-tang",
                        "Đời sống",
                        "Một hộp quà đẹp bắt đầu từ chất giấy, nếp gấp và màu mực — những chi tiết người nhận cảm nhận trước cả khi mở ra.",
                        "/images/placeholders/blog-4.jpg",
                        false,
                        new DateTime(2026, 4, 9, 0, 0, 0, DateTimeKind.Utc)
                    },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (var id = 20; id <= 22; id++)
            {
                migrationBuilder.DeleteData(table: "BlogPosts", keyColumn: "Id", keyValue: id);
            }
        }
    }
}
