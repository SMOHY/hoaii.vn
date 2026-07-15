using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Web.Services;

/// <summary>Seeds the two demo vouchers the storefront used to hardcode, the first time the table
/// is empty. FREESHIP is now a real free-shipping voucher (it used to discount 0đ).</summary>
public static class VoucherSeeder
{
    public static async Task EnsureSeedAsync(HoaiiDbContext db)
    {
        if (await db.Vouchers.AnyAsync())
        {
            return;
        }

        db.Vouchers.AddRange(
            new Voucher { Code = "FREESHIP", Label = "Miễn phí vận chuyển", Tag = "Ưu đãi", Type = VoucherType.FreeShipping, IsActive = true },
            new Voucher { Code = "GIAM20", Label = "Giảm giá 20%", Tag = "Voucher", Type = VoucherType.Percentage, Value = 20m, IsActive = true });

        await db.SaveChangesAsync();
    }
}
