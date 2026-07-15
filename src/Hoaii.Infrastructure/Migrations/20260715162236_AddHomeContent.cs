using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hoaii.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHomeContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HomeAboutCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Caption = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageOnTop = table.Column<bool>(type: "bit", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeAboutCards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HomeBenefits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IconPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileLine1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileLine2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeBenefits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HomeCustomerLogos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogoKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeCustomerLogos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HomeFeaturedTiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsCard = table.Column<bool>(type: "bit", nullable: false),
                    AccentColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CollectionLabel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleLine1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditionLabel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HideOnMobile = table.Column<bool>(type: "bit", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeFeaturedTiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HomeHeroSlides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subtitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileSubtitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeHeroSlides", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HomeServiceTabs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconSvg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PanelImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaptionColorHex = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CtaText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CtaUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeServiceTabs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HomeAboutCards");

            migrationBuilder.DropTable(
                name: "HomeBenefits");

            migrationBuilder.DropTable(
                name: "HomeCustomerLogos");

            migrationBuilder.DropTable(
                name: "HomeFeaturedTiles");

            migrationBuilder.DropTable(
                name: "HomeHeroSlides");

            migrationBuilder.DropTable(
                name: "HomeServiceTabs");
        }
    }
}
