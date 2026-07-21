using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hoaii.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryBannerHero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BannerImageUrl",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeroStyle",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ParentLabel",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentUrl",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BannerImageUrl", "HeroStyle", "ParentLabel", "ParentUrl" },
                values: new object[] { null, 0, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BannerImageUrl", "HeroStyle", "ParentLabel", "ParentUrl" },
                values: new object[] { null, 0, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "BannerImageUrl", "HeroStyle", "ParentLabel", "ParentUrl" },
                values: new object[] { null, 0, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "BannerImageUrl", "HeroStyle", "ParentLabel", "ParentUrl" },
                values: new object[] { null, 0, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "BannerImageUrl", "HeroStyle", "ParentLabel", "ParentUrl" },
                values: new object[] { null, 0, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "BannerImageUrl", "HeroStyle", "ParentLabel", "ParentUrl" },
                values: new object[] { null, 0, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "BannerImageUrl", "HeroStyle", "ParentLabel", "ParentUrl" },
                values: new object[] { null, 0, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "BannerImageUrl", "HeroStyle", "ParentLabel", "ParentUrl" },
                values: new object[] { null, 0, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "BannerImageUrl", "HeroStyle", "ParentLabel", "ParentUrl" },
                values: new object[] { null, 0, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "BannerImageUrl", "HeroStyle", "ParentLabel", "ParentUrl" },
                values: new object[] { null, 0, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "BannerImageUrl", "HeroStyle", "ParentLabel", "ParentUrl" },
                values: new object[] { null, 0, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "BannerImageUrl", "HeroStyle", "ParentLabel", "ParentUrl" },
                values: new object[] { null, 0, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannerImageUrl",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "HeroStyle",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ParentLabel",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ParentUrl",
                table: "Categories");
        }
    }
}
