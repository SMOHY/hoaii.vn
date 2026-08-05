using Hoaii.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Hoaii.Infrastructure;

namespace Hoaii.Web.Services;

/// <summary>Seeds the 2 groups behind the "Quà theo dịp" chooser (see CategoryGroup,
/// OccasionController.Pages) and assigns the categories that were already hard-coded into each
/// page's Sections list — same grouping as before, just now a real, admin-editable relationship
/// instead of a C# array. Runs once; afterwards admin can rename groups or move categories
/// between them from the admin UI.</summary>
public static class CategoryGroupSeeder
{
    public static async Task EnsureSeedAsync(HoaiiDbContext db)
    {
        if (!await db.CategoryGroups.AnyAsync())
        {
            db.CategoryGroups.AddRange(
                new CategoryGroup { Name = "Quà tặng theo dịp", Route = "qua-theo-dip", SortOrder = 0 },
                new CategoryGroup { Name = "Quà tặng cá nhân", Route = "qua-tang-ca-nhan", SortOrder = 1 });
            await db.SaveChangesAsync();
        }

        var groups = await db.CategoryGroups.ToDictionaryAsync(g => g.Route);
        var assignments = new Dictionary<string, string>
        {
            ["ngay-le-tinh-yeu"] = "qua-theo-dip",
            ["ngay-quoc-te-phu-nu"] = "qua-theo-dip",
            ["qua-giang-sinh"] = "qua-theo-dip",
            ["qua-tang-nguoi-ay"] = "qua-tang-ca-nhan",
            ["qua-tang-bo-me"] = "qua-tang-ca-nhan",
        };

        var categories = await db.Categories.Where(c => assignments.Keys.Contains(c.Slug) && c.GroupId == null).ToListAsync();
        foreach (var category in categories)
        {
            if (groups.TryGetValue(assignments[category.Slug], out var group))
            {
                category.GroupId = group.Id;
            }
        }
        if (categories.Count > 0)
        {
            await db.SaveChangesAsync();
        }
    }
}
