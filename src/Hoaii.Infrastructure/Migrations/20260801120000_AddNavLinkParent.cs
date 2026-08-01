using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hoaii.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNavLinkParent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "NavLinks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NavLinks_ParentId",
                table: "NavLinks",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_NavLinks_NavLinks_ParentId",
                table: "NavLinks",
                column: "ParentId",
                principalTable: "NavLinks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NavLinks_NavLinks_ParentId",
                table: "NavLinks");

            migrationBuilder.DropIndex(
                name: "IX_NavLinks_ParentId",
                table: "NavLinks");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "NavLinks");
        }
    }
}
