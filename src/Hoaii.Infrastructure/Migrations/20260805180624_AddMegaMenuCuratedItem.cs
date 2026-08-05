using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hoaii.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMegaMenuCuratedItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MegaMenuCuratedItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PanelKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ColumnKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MegaMenuCuratedItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MegaMenuCuratedItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MegaMenuCuratedItems_PanelKey_ColumnKey_ProductId",
                table: "MegaMenuCuratedItems",
                columns: new[] { "PanelKey", "ColumnKey", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MegaMenuCuratedItems_PanelKey_ColumnKey_SortOrder",
                table: "MegaMenuCuratedItems",
                columns: new[] { "PanelKey", "ColumnKey", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_MegaMenuCuratedItems_ProductId",
                table: "MegaMenuCuratedItems",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MegaMenuCuratedItems");
        }
    }
}
