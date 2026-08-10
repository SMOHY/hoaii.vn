using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hoaii.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCollectionCms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Collections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeroEyebrow",
                table: "Collections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeroKicker",
                table: "Collections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromoBackground",
                table: "Collections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromoCtaText",
                table: "Collections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromoCtaUrl",
                table: "Collections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromoEyebrow",
                table: "Collections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromoImageUrl",
                table: "Collections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromoTitle",
                table: "Collections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PromoWide",
                table: "Collections",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Collections",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            // Existing rows all land on "" above, which would collide on the unique index below.
            // Give each a distinct placeholder slug from its Id — an admin can rename it to
            // something readable from Areas/Admin/Views/Collections/Edit.cshtml afterward.
            // Wrapped in EXEC() so SQL Server binds the column reference at execution time rather
            // than at parse time of this whole idempotent-script batch, where the AddColumn above
            // hasn't taken effect yet ("Invalid column name 'Slug'").
            migrationBuilder.Sql(
                "EXEC(N'UPDATE [Collections] SET [Slug] = ''bo-suu-tap-'' + CAST([Id] AS nvarchar(10)) WHERE [Slug] = ''''')");

            migrationBuilder.CreateTable(
                name: "CollectionHeroSlides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CollectionId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LinkUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionHeroSlides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollectionHeroSlides_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Collections_Slug",
                table: "Collections",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CollectionHeroSlides_CollectionId",
                table: "CollectionHeroSlides",
                column: "CollectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollectionHeroSlides");

            migrationBuilder.DropIndex(
                name: "IX_Collections_Slug",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "HeroEyebrow",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "HeroKicker",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "PromoBackground",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "PromoCtaText",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "PromoCtaUrl",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "PromoEyebrow",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "PromoImageUrl",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "PromoTitle",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "PromoWide",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Collections");
        }
    }
}
