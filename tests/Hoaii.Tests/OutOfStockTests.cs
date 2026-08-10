using Hoaii.Domain.Entities;
using Hoaii.Infrastructure;
using Hoaii.Web.Controllers;
using Hoaii.Web.Models.Product;
using Hoaii.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Tests;

/// <summary>
/// Sản phẩm gắn badge "Hết hàng" phải không mua được ở MỌI cửa, không chỉ ở nút "+" trên lưới:
/// trang chi tiết, thêm vào giỏ, giỏ hàng, và đặt hàng.
/// </summary>
public class OutOfStockTests
{
    private const int OutOfStockProductId = 101;
    private const int InStockProductId = 102;
    private const int OutOfStockVariantId = 1001;

    private static HoaiiDbContext NewDb()
    {
        // Tên DB khác nhau mỗi test để các test không dùng chung state.
        var options = new DbContextOptionsBuilder<HoaiiDbContext>()
            .UseInMemoryDatabase($"oos-{Guid.NewGuid()}")
            .Options;

        var db = new HoaiiDbContext(options);
        db.Categories.Add(new Category { Id = 9, Name = "Trà", Slug = "tra" });
        db.Products.Add(new Product
        {
            Id = OutOfStockProductId,
            Name = "Trà sen vàng",
            Slug = "tra-sen-vang",
            Price = 250_000m,
            CategoryId = 9,
            Badge = ProductBadge.OutOfStock,
            Variants = [new ProductVariant { Id = OutOfStockVariantId, Name = "Hộp 4 túi / màu vàng" }],
        });
        db.Products.Add(new Product
        {
            Id = InStockProductId,
            Name = "Trà lài",
            Slug = "tra-lai",
            Price = 190_000m,
            CategoryId = 9,
            Badge = ProductBadge.None,
        });
        db.SaveChanges();
        return db;
    }

    private static CartService NewCart(HoaiiDbContext db)
    {
        var http = new DefaultHttpContext { Session = new FakeSession() };
        return new CartService(new HttpContextAccessor { HttpContext = http }, db);
    }

    [Fact]
    public async Task Trang_chi_tiet_danh_dau_san_pham_het_hang()
    {
        using var db = NewDb();

        var result = await new ProductController(db).Details("tra-sen-vang");

        var model = Assert.IsType<ProductDetailsViewModel>(Assert.IsType<ViewResult>(result).Model);
        Assert.True(model.IsOutOfStock);
    }

    [Fact]
    public async Task Them_vao_gio_bi_tu_choi_khi_het_hang()
    {
        using var db = NewDb();
        var cart = NewCart(db);

        // Kiểm tra riêng giá trị trả về: nếu chỉ xét giỏ rỗng thì bộ lọc ở GetCartAsync sẽ che mất
        // việc chốt chặn tại AddItemAsync có hoạt động hay không.
        var accepted = await cart.AddItemAsync(OutOfStockProductId, OutOfStockVariantId, 1);

        Assert.False(accepted);
        Assert.Empty((await cart.GetCartAsync()).Items);
    }

    [Fact]
    public async Task San_pham_dat_het_hang_sau_khi_da_vao_gio_thi_roi_khoi_gio()
    {
        using var db = NewDb();
        var cart = NewCart(db);

        // Vào giỏ lúc còn hàng…
        await cart.AddItemAsync(InStockProductId, null, 2);
        Assert.Single((await cart.GetCartAsync()).Items);

        // …rồi shop gắn "Hết hàng".
        var product = await db.Products.FirstAsync(p => p.Id == InStockProductId);
        product.Badge = ProductBadge.OutOfStock;
        await db.SaveChangesAsync();

        Assert.Empty((await cart.GetCartAsync()).Items);
    }

    [Fact]
    public async Task Hang_con_ban_thi_van_them_vao_gio_binh_thuong()
    {
        using var db = NewDb();
        var cart = NewCart(db);

        await cart.AddItemAsync(InStockProductId, null, 1);

        var item = Assert.Single((await cart.GetCartAsync()).Items);
        Assert.Equal(InStockProductId, item.ProductId);
    }
}

/// <summary>ISession tối giản chạy trên Dictionary — CartService chỉ đọc/ghi một khoá chuỗi.</summary>
internal sealed class FakeSession : ISession
{
    private readonly Dictionary<string, byte[]> _store = [];

    public bool IsAvailable => true;
    public string Id => "test-session";
    public IEnumerable<string> Keys => _store.Keys;

    public void Clear() => _store.Clear();
    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void Remove(string key) => _store.Remove(key);
    public void Set(string key, byte[] value) => _store[key] = value;
    public bool TryGetValue(string key, out byte[]? value) => _store.TryGetValue(key, out value);
}
