using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hoaii.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHomeCustomerLogoImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "HomeCustomerLogos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "HomeCustomerLogos");
        }
    }
}
