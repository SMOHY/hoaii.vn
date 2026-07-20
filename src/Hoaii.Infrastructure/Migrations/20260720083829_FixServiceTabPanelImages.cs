using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hoaii.Infrastructure.Migrations
{
    /// <summary>
    /// The three home service tabs used to share one photo, <c>service-panel.jpg</c>. That file was
    /// replaced by a per-tab photo and deleted from wwwroot, and the seeder was updated to match —
    /// but the seeder only runs when <c>HomeServiceTabs</c> is empty, so every database that had
    /// already been seeded kept pointing at the deleted file and served a 404 on the home page.
    /// </summary>
    public partial class FixServiceTabPanelImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Matched on the stale path as well as the key, so a photo an admin has since chosen
            // through the CMS is never overwritten.
            migrationBuilder.Sql(@"
                UPDATE HomeServiceTabs SET PanelImageUrl = '/images/home/service-in-khac.jpg'
                WHERE [Key] = 'in-khac'  AND PanelImageUrl = '/images/home/service-panel.jpg';

                UPDATE HomeServiceTabs SET PanelImageUrl = '/images/home/service-goi-qua.jpg'
                WHERE [Key] = 'goi-qua'  AND PanelImageUrl = '/images/home/service-panel.jpg';

                UPDATE HomeServiceTabs SET PanelImageUrl = '/images/home/service-thiet-ke.jpg'
                WHERE [Key] = 'thiet-ke' AND PanelImageUrl = '/images/home/service-panel.jpg';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Deliberately empty: service-panel.jpg no longer exists in wwwroot, so putting the old
            // path back would only reintroduce the 404 this migration exists to remove.
        }
    }
}
