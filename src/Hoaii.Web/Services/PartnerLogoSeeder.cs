using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Services;

/// <summary>Seeds the 24 partner logos the Hợp tác page used to hardcode, the first time empty.</summary>
public static class PartnerLogoSeeder
{
    public static async Task EnsureSeedAsync(HoaiiDbContext db)
    {
        if (await db.PartnerLogos.AnyAsync()) return;

        string[] logos =
        [
            "hakyo", "mb", "truong-thanh", "bondex", "nano-gold", "pro-group", "jaguar", "bee-mv",
            "isocial", "core5", "cystack", "arent", "nature-hotel", "100bold", "eventista", "gosu",
            "avalue", "isofh", "deli", "winggo", "kleur", "eysin", "nam-viet", "happy-sun",
        ];
        for (var i = 0; i < logos.Length; i++)
        {
            db.PartnerLogos.Add(new PartnerLogo { LogoKey = logos[i], SortOrder = i });
        }
        await db.SaveChangesAsync();
    }
}
