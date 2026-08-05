using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hoaii.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMegaMenuColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MegaMenuColumns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PanelKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Kind = table.Column<int>(type: "int", nullable: false),
                    CollectionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MegaMenuColumns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MegaMenuColumns_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "MegaMenuColumnItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MegaMenuColumnId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MegaMenuColumnItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MegaMenuColumnItems_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MegaMenuColumnItems_MegaMenuColumns_MegaMenuColumnId",
                        column: x => x.MegaMenuColumnId,
                        principalTable: "MegaMenuColumns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MegaMenuColumnItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MegaMenuColumnItems_CategoryId",
                table: "MegaMenuColumnItems",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MegaMenuColumnItems_MegaMenuColumnId",
                table: "MegaMenuColumnItems",
                column: "MegaMenuColumnId");

            migrationBuilder.CreateIndex(
                name: "IX_MegaMenuColumnItems_ProductId",
                table: "MegaMenuColumnItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_MegaMenuColumns_CollectionId",
                table: "MegaMenuColumns",
                column: "CollectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MegaMenuColumnItems");

            migrationBuilder.DropTable(
                name: "MegaMenuColumns");
        }
    }
}
