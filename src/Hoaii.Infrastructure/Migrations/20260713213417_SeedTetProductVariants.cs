using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hoaii.Infrastructure.Migrations
{
    /// <summary>
    /// Figma's product card carries a row of colour swatches under the price (node 1519:34033),
    /// and the PDP offers a "LOẠI HỘP" choice (node 826:20654). Both are driven by
    /// ProductVariants, which the Tết products didn't have — so the swatches never rendered
    /// and the card came out 26px shorter than the design.
    /// </summary>
    public partial class SeedTetProductVariants : Migration
    {
        private const int FirstVariantId = 100;

        // Product ids seeded in SeedTetProductsFromFigma.
        private static readonly int[] TetProductIds = [20, 21, 30, 31, 32, 33];

        // (Name, price delta) — same shape as the existing seed: "Hộp 4 túi / màu vàng".
        private static readonly (string Name, decimal Delta)[] Options =
        [
            ("Hộp 4 bánh / màu vàng", 0m),
            ("Hộp 6 bánh / màu đỏ", 200000m),
        ];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var id = FirstVariantId;

            foreach (var productId in TetProductIds)
            {
                foreach (var (name, delta) in Options)
                {
                    migrationBuilder.InsertData(
                        table: "ProductVariants",
                        columns: new[] { "Id", "Name", "PriceModifier", "ProductId", "Sku", "StockQuantity" },
                        values: new object[] { id++, name, delta, productId, null, 50 });
                }
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var total = TetProductIds.Length * Options.Length;
            for (var i = 0; i < total; i++)
            {
                migrationBuilder.DeleteData(table: "ProductVariants", keyColumn: "Id", keyValue: FirstVariantId + i);
            }
        }
    }
}
