using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hoaii.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNavAndCmsFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FeatureBody",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureImageUrl",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureTitle",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoryBody",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoryImageUrl",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoryTitle",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeroEyebrow",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromoCtaText",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromoCtaUrl",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromoEyebrow",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromoImageUrl",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromoTitle",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FooterMenuColumns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FooterMenuColumns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NavLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Placement = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HasDropdown = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NavLinks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FooterMenuLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FooterMenuColumnId = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FooterMenuLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FooterMenuLinks_FooterMenuColumns_FooterMenuColumnId",
                        column: x => x.FooterMenuColumnId,
                        principalTable: "FooterMenuColumns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "HeroEyebrow", "PromoCtaText", "PromoCtaUrl", "PromoEyebrow", "PromoImageUrl", "PromoTitle" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "HeroEyebrow", "PromoCtaText", "PromoCtaUrl", "PromoEyebrow", "PromoImageUrl", "PromoTitle" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "HeroEyebrow", "PromoCtaText", "PromoCtaUrl", "PromoEyebrow", "PromoImageUrl", "PromoTitle" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "HeroEyebrow", "PromoCtaText", "PromoCtaUrl", "PromoEyebrow", "PromoImageUrl", "PromoTitle" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "HeroEyebrow", "PromoCtaText", "PromoCtaUrl", "PromoEyebrow", "PromoImageUrl", "PromoTitle" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Description", "HeroEyebrow", "PromoCtaText", "PromoCtaUrl", "PromoEyebrow", "PromoImageUrl", "PromoTitle" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Description", "HeroEyebrow", "PromoCtaText", "PromoCtaUrl", "PromoEyebrow", "PromoImageUrl", "PromoTitle" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Description", "HeroEyebrow", "PromoCtaText", "PromoCtaUrl", "PromoEyebrow", "PromoImageUrl", "PromoTitle" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Description", "HeroEyebrow", "PromoCtaText", "PromoCtaUrl", "PromoEyebrow", "PromoImageUrl", "PromoTitle" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Description", "HeroEyebrow", "PromoCtaText", "PromoCtaUrl", "PromoEyebrow", "PromoImageUrl", "PromoTitle" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Description", "HeroEyebrow", "PromoCtaText", "PromoCtaUrl", "PromoEyebrow", "PromoImageUrl", "PromoTitle" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Description", "HeroEyebrow", "PromoCtaText", "PromoCtaUrl", "PromoEyebrow", "PromoImageUrl", "PromoTitle" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FeatureBody", "FeatureImageUrl", "FeatureTitle", "StoryBody", "StoryImageUrl", "StoryTitle" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FeatureBody", "FeatureImageUrl", "FeatureTitle", "StoryBody", "StoryImageUrl", "StoryTitle" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FeatureBody", "FeatureImageUrl", "FeatureTitle", "StoryBody", "StoryImageUrl", "StoryTitle" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FeatureBody", "FeatureImageUrl", "FeatureTitle", "StoryBody", "StoryImageUrl", "StoryTitle" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "FeatureBody", "FeatureImageUrl", "FeatureTitle", "StoryBody", "StoryImageUrl", "StoryTitle" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "FeatureBody", "FeatureImageUrl", "FeatureTitle", "StoryBody", "StoryImageUrl", "StoryTitle" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "FeatureBody", "FeatureImageUrl", "FeatureTitle", "StoryBody", "StoryImageUrl", "StoryTitle" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "FeatureBody", "FeatureImageUrl", "FeatureTitle", "StoryBody", "StoryImageUrl", "StoryTitle" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "FeatureBody", "FeatureImageUrl", "FeatureTitle", "StoryBody", "StoryImageUrl", "StoryTitle" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "FeatureBody", "FeatureImageUrl", "FeatureTitle", "StoryBody", "StoryImageUrl", "StoryTitle" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "FeatureBody", "FeatureImageUrl", "FeatureTitle", "StoryBody", "StoryImageUrl", "StoryTitle" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "FeatureBody", "FeatureImageUrl", "FeatureTitle", "StoryBody", "StoryImageUrl", "StoryTitle" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_FooterMenuLinks_FooterMenuColumnId",
                table: "FooterMenuLinks",
                column: "FooterMenuColumnId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FooterMenuLinks");

            migrationBuilder.DropTable(
                name: "NavLinks");

            migrationBuilder.DropTable(
                name: "FooterMenuColumns");

            migrationBuilder.DropColumn(
                name: "FeatureBody",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "FeatureImageUrl",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "FeatureTitle",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StoryBody",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StoryImageUrl",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StoryTitle",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "HeroEyebrow",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "PromoCtaText",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "PromoCtaUrl",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "PromoEyebrow",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "PromoImageUrl",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "PromoTitle",
                table: "Categories");
        }
    }
}
