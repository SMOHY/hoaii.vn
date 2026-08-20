using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hoaii.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AnhRiengChoMobileVaDiemNeo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FeatureImageFocal",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureImageUrlMobile",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoryImageFocal",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoryImageUrlMobile",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageFocal",
                table: "HomeHeroSlides",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileImageUrl",
                table: "HomeHeroSlides",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromoImageFocal",
                table: "Collections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromoImageUrlMobile",
                table: "Collections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageFocal",
                table: "CollectionHeroSlides",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileImageUrl",
                table: "CollectionHeroSlides",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageFocal",
                table: "CategoryHeroSlides",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileImageUrl",
                table: "CategoryHeroSlides",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BannerImageFocal",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BannerImageUrlMobile",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverImageFocal",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverImageUrlMobile",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromoImageFocal",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromoImageUrlMobile",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BannerImageFocal", "BannerImageUrlMobile", "CoverImageFocal", "CoverImageUrlMobile", "PromoImageFocal", "PromoImageUrlMobile" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BannerImageFocal", "BannerImageUrlMobile", "CoverImageFocal", "CoverImageUrlMobile", "PromoImageFocal", "PromoImageUrlMobile" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "BannerImageFocal", "BannerImageUrlMobile", "CoverImageFocal", "CoverImageUrlMobile", "PromoImageFocal", "PromoImageUrlMobile" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "BannerImageFocal", "BannerImageUrlMobile", "CoverImageFocal", "CoverImageUrlMobile", "PromoImageFocal", "PromoImageUrlMobile" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "BannerImageFocal", "BannerImageUrlMobile", "CoverImageFocal", "CoverImageUrlMobile", "PromoImageFocal", "PromoImageUrlMobile" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "BannerImageFocal", "BannerImageUrlMobile", "CoverImageFocal", "CoverImageUrlMobile", "PromoImageFocal", "PromoImageUrlMobile" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "BannerImageFocal", "BannerImageUrlMobile", "CoverImageFocal", "CoverImageUrlMobile", "PromoImageFocal", "PromoImageUrlMobile" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "BannerImageFocal", "BannerImageUrlMobile", "CoverImageFocal", "CoverImageUrlMobile", "PromoImageFocal", "PromoImageUrlMobile" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "BannerImageFocal", "BannerImageUrlMobile", "CoverImageFocal", "CoverImageUrlMobile", "PromoImageFocal", "PromoImageUrlMobile" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "BannerImageFocal", "BannerImageUrlMobile", "CoverImageFocal", "CoverImageUrlMobile", "PromoImageFocal", "PromoImageUrlMobile" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "BannerImageFocal", "BannerImageUrlMobile", "CoverImageFocal", "CoverImageUrlMobile", "PromoImageFocal", "PromoImageUrlMobile" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "BannerImageFocal", "BannerImageUrlMobile", "CoverImageFocal", "CoverImageUrlMobile", "PromoImageFocal", "PromoImageUrlMobile" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FeatureImageFocal", "FeatureImageUrlMobile", "StoryImageFocal", "StoryImageUrlMobile" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FeatureImageFocal", "FeatureImageUrlMobile", "StoryImageFocal", "StoryImageUrlMobile" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FeatureImageFocal", "FeatureImageUrlMobile", "StoryImageFocal", "StoryImageUrlMobile" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FeatureImageFocal", "FeatureImageUrlMobile", "StoryImageFocal", "StoryImageUrlMobile" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "FeatureImageFocal", "FeatureImageUrlMobile", "StoryImageFocal", "StoryImageUrlMobile" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "FeatureImageFocal", "FeatureImageUrlMobile", "StoryImageFocal", "StoryImageUrlMobile" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "FeatureImageFocal", "FeatureImageUrlMobile", "StoryImageFocal", "StoryImageUrlMobile" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "FeatureImageFocal", "FeatureImageUrlMobile", "StoryImageFocal", "StoryImageUrlMobile" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "FeatureImageFocal", "FeatureImageUrlMobile", "StoryImageFocal", "StoryImageUrlMobile" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "FeatureImageFocal", "FeatureImageUrlMobile", "StoryImageFocal", "StoryImageUrlMobile" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "FeatureImageFocal", "FeatureImageUrlMobile", "StoryImageFocal", "StoryImageUrlMobile" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "FeatureImageFocal", "FeatureImageUrlMobile", "StoryImageFocal", "StoryImageUrlMobile" },
                values: new object[] { null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeatureImageFocal",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "FeatureImageUrlMobile",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StoryImageFocal",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StoryImageUrlMobile",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ImageFocal",
                table: "HomeHeroSlides");

            migrationBuilder.DropColumn(
                name: "MobileImageUrl",
                table: "HomeHeroSlides");

            migrationBuilder.DropColumn(
                name: "PromoImageFocal",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "PromoImageUrlMobile",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "ImageFocal",
                table: "CollectionHeroSlides");

            migrationBuilder.DropColumn(
                name: "MobileImageUrl",
                table: "CollectionHeroSlides");

            migrationBuilder.DropColumn(
                name: "ImageFocal",
                table: "CategoryHeroSlides");

            migrationBuilder.DropColumn(
                name: "MobileImageUrl",
                table: "CategoryHeroSlides");

            migrationBuilder.DropColumn(
                name: "BannerImageFocal",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "BannerImageUrlMobile",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CoverImageFocal",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CoverImageUrlMobile",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "PromoImageFocal",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "PromoImageUrlMobile",
                table: "Categories");
        }
    }
}
