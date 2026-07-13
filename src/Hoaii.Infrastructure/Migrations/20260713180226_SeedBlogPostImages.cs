using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hoaii.Infrastructure.Migrations
{
    /// <summary>
    /// The seeded posts went in with a null ImageUrl, so every blog card — on /blog and in the
    /// Home "HOÀI MÁCH BẠN" strip — rendered as an empty box. Point them at the article artwork.
    /// </summary>
    public partial class SeedBlogPostImages : Migration
    {
        private static readonly (int Id, string ImageUrl)[] Images =
        [
            (1, "/images/placeholders/blog-1.jpg"),
            (2, "/images/placeholders/blog-2.jpg"),
            (3, "/images/placeholders/blog-3.jpg"),
            (4, "/images/placeholders/blog-4.jpg"),
        ];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var (id, imageUrl) in Images)
            {
                migrationBuilder.UpdateData(
                    table: "BlogPosts",
                    keyColumn: "Id",
                    keyValue: id,
                    column: "ImageUrl",
                    value: imageUrl);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var (id, _) in Images)
            {
                migrationBuilder.UpdateData(
                    table: "BlogPosts",
                    keyColumn: "Id",
                    keyValue: id,
                    column: "ImageUrl",
                    value: null);
            }
        }
    }
}
