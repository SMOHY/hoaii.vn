using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hoaii.Infrastructure.Migrations
{
    /// <summary>
    /// The PDP prints the variant name straight onto the "LOẠI HỘP" buttons, which Figma
    /// sizes at 160x48 with a single short label (node 826:20657). The seeded names were
    /// long enough to wrap onto two lines, so they are shortened to match.
    /// </summary>
    public partial class RenameTetVariantsToFigmaLabels : Migration
    {
        private const int FirstVariantId = 100;
        private const int VariantCount = 12; // 6 Tết products x 2 options

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ids alternate: even = first option, odd = second (see SeedTetProductVariants).
            for (var i = 0; i < VariantCount; i++)
            {
                migrationBuilder.UpdateData(
                    table: "ProductVariants",
                    keyColumn: "Id",
                    keyValue: FirstVariantId + i,
                    column: "Name",
                    value: i % 2 == 0 ? "4 Bánh" : "6 Bánh");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (var i = 0; i < VariantCount; i++)
            {
                migrationBuilder.UpdateData(
                    table: "ProductVariants",
                    keyColumn: "Id",
                    keyValue: FirstVariantId + i,
                    column: "Name",
                    value: i % 2 == 0 ? "Hộp 4 bánh / màu vàng" : "Hộp 6 bánh / màu đỏ");
            }
        }
    }
}
