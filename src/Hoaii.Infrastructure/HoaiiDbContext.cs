using Hoaii.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hoaii.Infrastructure;

public class HoaiiDbContext(DbContextOptions<HoaiiDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Province> Provinces => Set<Province>();
    public DbSet<Ward> Wards => Set<Ward>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();

    // Admin area.
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<AdminAuditLog> AdminAuditLogs => Set<AdminAuditLog>();
    public DbSet<OrderStatusHistory> OrderStatusHistories => Set<OrderStatusHistory>();
    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();

    // CMS.
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();
    public DbSet<PolicyPage> PolicyPages => Set<PolicyPage>();
    public DbSet<PolicyBlock> PolicyBlocks => Set<PolicyBlock>();
    public DbSet<HomeHeroSlide> HomeHeroSlides => Set<HomeHeroSlide>();
    public DbSet<HomeBenefit> HomeBenefits => Set<HomeBenefit>();
    public DbSet<HomeFeaturedTile> HomeFeaturedTiles => Set<HomeFeaturedTile>();
    public DbSet<HomeServiceTab> HomeServiceTabs => Set<HomeServiceTab>();
    public DbSet<HomeAboutCard> HomeAboutCards => Set<HomeAboutCard>();
    public DbSet<HomeCustomerLogo> HomeCustomerLogos => Set<HomeCustomerLogo>();
    public DbSet<NavLink> NavLinks => Set<NavLink>();
    public DbSet<FooterMenuColumn> FooterMenuColumns => Set<FooterMenuColumn>();
    public DbSet<FooterMenuLink> FooterMenuLinks => Set<FooterMenuLink>();
    public DbSet<Voucher> Vouchers => Set<Voucher>();
    public DbSet<ContactSubmission> ContactSubmissions => Set<ContactSubmission>();
    public DbSet<WholesaleLead> WholesaleLeads => Set<WholesaleLead>();
    public DbSet<NewsletterSubscriber> NewsletterSubscribers => Set<NewsletterSubscriber>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasIndex(c => c.Slug).IsUnique();
            entity.Property(c => c.Name).HasMaxLength(200);
            entity.Property(c => c.Slug).HasMaxLength(200);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(p => p.Slug).IsUnique();
            entity.Property(p => p.Name).HasMaxLength(300);
            entity.Property(p => p.Slug).HasMaxLength(300);
            entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            entity.Property(p => p.CompareAtPrice).HasColumnType("decimal(18,2)");

            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasOne(i => i.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.Property(v => v.PriceModifier).HasColumnType("decimal(18,2)");
            entity.HasOne(v => v.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasIndex(o => o.OrderNumber).IsUnique();
            entity.Property(o => o.Subtotal).HasColumnType("decimal(18,2)");
            entity.Property(o => o.ShippingFee).HasColumnType("decimal(18,2)");
            entity.Property(o => o.Discount).HasColumnType("decimal(18,2)");
            entity.Property(o => o.Total).HasColumnType("decimal(18,2)");
            entity.Property(o => o.VoucherCode).HasMaxLength(50);
            entity.Property(o => o.TrackingNumber).HasMaxLength(100);

            // The admin order list filters and sorts on these two constantly.
            entity.HasIndex(o => o.Status);
            entity.HasIndex(o => o.CreatedAt);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
            entity.HasOne(i => i.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasIndex(c => c.Email).IsUnique();
            entity.Property(c => c.Email).HasMaxLength(320);
        });

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Province>(entity =>
        {
            entity.Property(p => p.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<Ward>(entity =>
        {
            entity.Property(w => w.Name).HasMaxLength(200);
            entity.HasOne(w => w.Province)
                .WithMany(p => p.Wards)
                .HasForeignKey(w => w.ProvinceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasOne(a => a.Customer)
                .WithMany(c => c.Addresses)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(a => a.Province)
                .WithMany()
                .HasForeignKey(a => a.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(a => a.Ward)
                .WithMany()
                .HasForeignKey(a => a.WardId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.HasIndex(b => b.Slug).IsUnique();
            entity.Property(b => b.Title).HasMaxLength(300);
            entity.Property(b => b.Slug).HasMaxLength(300);
            entity.Property(b => b.Category).HasMaxLength(100);
            entity.Property(b => b.Author).HasMaxLength(150);
        });

        modelBuilder.Entity<AdminUser>(entity =>
        {
            entity.HasIndex(a => a.Email).IsUnique();
            entity.Property(a => a.Email).HasMaxLength(320);
            entity.Property(a => a.FullName).HasMaxLength(200);
            entity.Property(a => a.PasswordHash).HasMaxLength(500);
        });

        modelBuilder.Entity<OrderStatusHistory>(entity =>
        {
            entity.HasOne(h => h.Order)
                .WithMany(o => o.StatusHistory)
                .HasForeignKey(h => h.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Keep the history if the admin who made the change is later removed.
            entity.HasOne(h => h.AdminUser)
                .WithMany()
                .HasForeignKey(h => h.AdminUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<AdminAuditLog>(entity =>
        {
            entity.Property(l => l.Action).HasMaxLength(100);
            entity.Property(l => l.EntityType).HasMaxLength(100);
            entity.HasIndex(l => l.CreatedAt);

            entity.HasOne(l => l.AdminUser)
                .WithMany()
                .HasForeignKey(l => l.AdminUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<MediaAsset>(entity =>
        {
            entity.Property(m => m.Url).HasMaxLength(500);
            entity.Property(m => m.FileName).HasMaxLength(300);
            entity.HasIndex(m => m.CreatedAt);

            entity.HasOne(m => m.UploadedByAdminUser)
                .WithMany()
                .HasForeignKey(m => m.UploadedByAdminUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<SiteSetting>(entity =>
        {
            entity.HasKey(s => s.Key);
            entity.Property(s => s.Key).HasMaxLength(100);
            entity.Property(s => s.Value).HasMaxLength(2000);
        });

        modelBuilder.Entity<PolicyPage>(entity =>
        {
            entity.HasIndex(p => p.Slug).IsUnique();
            entity.Property(p => p.Slug).HasMaxLength(120);
            entity.Property(p => p.Title).HasMaxLength(300);
            entity.Property(p => p.NavLabel).HasMaxLength(200);
            entity.Property(p => p.BreadcrumbLabel).HasMaxLength(300);

            entity.HasMany(p => p.Blocks)
                .WithOne(b => b.PolicyPage!)
                .HasForeignKey(b => b.PolicyPageId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ContactSubmission>(e => { e.Property(x => x.Email).HasMaxLength(320); e.HasIndex(x => x.CreatedAt); });
        modelBuilder.Entity<WholesaleLead>(e => { e.Property(x => x.Email).HasMaxLength(320); e.HasIndex(x => x.CreatedAt); });
        modelBuilder.Entity<NewsletterSubscriber>(e => { e.Property(x => x.Email).HasMaxLength(320); e.HasIndex(x => x.Email).IsUnique(); });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasIndex(v => v.Code).IsUnique();
            entity.Property(v => v.Code).HasMaxLength(50);
            entity.Property(v => v.Label).HasMaxLength(200);
            entity.Property(v => v.Tag).HasMaxLength(50);
            entity.Property(v => v.Value).HasColumnType("decimal(18,2)");
            entity.Property(v => v.MinOrderAmount).HasColumnType("decimal(18,2)");
            entity.Property(v => v.MaxDiscountAmount).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<FooterMenuColumn>(entity =>
        {
            entity.HasMany(c => c.Links)
                .WithOne(l => l.Column!)
                .HasForeignKey(l => l.FooterMenuColumnId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        SeedCategories(modelBuilder);
        SeedProducts(modelBuilder);
        SeedProductVariants(modelBuilder);
        SeedGeo(modelBuilder);
        SeedBlogPosts(modelBuilder);
    }

    private static void SeedBlogPosts(ModelBuilder modelBuilder)
    {
        var posts = new[]
        {
            new BlogPost
            {
                Id = 1, Title = "Gợi ý chọn quà tặng cho người thân yêu", Slug = "goi-y-chon-qua-tang-nguoi-than",
                Category = "Đời sống", IsFeatured = true, PublishedAt = new DateTime(2026, 7, 2),
                Excerpt = "Chọn quà tặng sao cho vừa ý nghĩa vừa tinh tế luôn là điều khiến nhiều người trăn trở. Cùng HOÀI khám phá những gợi ý quà tặng phù hợp với từng đối tượng và dịp lễ trong năm.",
            },
            new BlogPost
            {
                Id = 2, Title = "Trà sen Tây Hồ — tinh hoa trà Việt trăm năm", Slug = "tra-sen-tay-ho-tinh-hoa-tra-viet",
                Category = "Văn hóa", PublishedAt = new DateTime(2026, 6, 20),
                Excerpt = "Khám phá quy trình ướp trà sen truyền thống của người Hà Nội, một nét đẹp văn hóa được gìn giữ qua nhiều thế hệ.",
            },
            new BlogPost
            {
                Id = 3, Title = "5 mẫu hộp quà Tết được yêu thích nhất 2026", Slug = "5-mau-hop-qua-tet-yeu-thich-2026",
                Category = "Xu hướng", PublishedAt = new DateTime(2026, 6, 10),
                Excerpt = "Tổng hợp những mẫu hộp quà Tết bán chạy nhất mùa Tết 2026 tại HOÀI, phù hợp biếu tặng đối tác và người thân.",
            },
            new BlogPost
            {
                Id = 4, Title = "Nghệ thuật gói quà kiểu Nhật Furoshiki", Slug = "nghe-thuat-goi-qua-furoshiki",
                Category = "Đời sống", PublishedAt = new DateTime(2026, 5, 28),
                Excerpt = "Furoshiki không chỉ là cách gói quà mà còn là một nghệ thuật thể hiện sự trân trọng dành cho người nhận.",
            },
        };

        modelBuilder.Entity<BlogPost>().HasData(posts);
    }

    private static void SeedCategories(ModelBuilder modelBuilder)
    {
        var categories = new[]
        {
            new Category { Id = 1, Name = "Trà", Slug = "tra", Type = CategoryType.ProductType, SortOrder = 1 },
            new Category { Id = 2, Name = "Khăn", Slug = "khan", Type = CategoryType.ProductType, SortOrder = 2 },
            new Category { Id = 3, Name = "Tượng gốm", Slug = "tuong-gom", Type = CategoryType.ProductType, SortOrder = 3 },
            new Category { Id = 4, Name = "Rượu", Slug = "ruou", Type = CategoryType.ProductType, SortOrder = 4 },
            new Category { Id = 5, Name = "Quà tết", Slug = "qua-tet", Type = CategoryType.Occasion, SortOrder = 5 },
            new Category { Id = 6, Name = "Quà trung thu", Slug = "qua-trung-thu", Type = CategoryType.Occasion, SortOrder = 6 },
            new Category { Id = 7, Name = "Quà giáng sinh", Slug = "qua-giang-sinh", Type = CategoryType.Occasion, SortOrder = 7 },
            new Category { Id = 8, Name = "Quà tặng bố mẹ", Slug = "qua-tang-bo-me", Type = CategoryType.Occasion, SortOrder = 8 },
            new Category { Id = 9, Name = "Quà tặng người ấy", Slug = "qua-tang-nguoi-ay", Type = CategoryType.Occasion, SortOrder = 9 },
            new Category { Id = 10, Name = "Ngày quốc tế phụ nữ", Slug = "ngay-quoc-te-phu-nu", Type = CategoryType.Occasion, SortOrder = 10 },
            new Category { Id = 11, Name = "Ngày lễ tình yêu", Slug = "ngay-le-tinh-yeu", Type = CategoryType.Occasion, SortOrder = 11 },
            new Category { Id = 12, Name = "Quà tặng theo dịp", Slug = "qua-tang-theo-dip", Type = CategoryType.Occasion, SortOrder = 12 },
        };

        modelBuilder.Entity<Category>().HasData(categories);
    }

    private static void SeedProducts(ModelBuilder modelBuilder)
    {
        // Sample data for smoke-testing the DB connection and Category page — replace with real catalog data.
        var products = new[]
        {
            new Product { Id = 1, Name = "Trà sen vàng", Slug = "tra-sen-vang", Price = 250_000m, CategoryId = 1, Badge = ProductBadge.New },
            new Product { Id = 2, Name = "Trà ô long thượng hạng", Slug = "tra-o-long-thuong-hang", Price = 320_000m, CompareAtPrice = 380_000m, CategoryId = 1, Badge = ProductBadge.Sale },
            new Product { Id = 3, Name = "Trà sen Tây Hồ hộp gỗ", Slug = "tra-sen-tay-ho-hop-go", Price = 410_000m, CategoryId = 1 },
            new Product { Id = 4, Name = "Trà shan tuyết cổ thụ", Slug = "tra-shan-tuyet-co-thu", Price = 550_000m, CategoryId = 1, Badge = ProductBadge.New },
            new Product { Id = 5, Name = "Trà lài ướp hương", Slug = "tra-lai-uop-huong", Price = 190_000m, CategoryId = 1, Badge = ProductBadge.OutOfStock },
            new Product { Id = 6, Name = "Trà đen kỵ sữa", Slug = "tra-den-ky-sua", Price = 220_000m, CategoryId = 1 },
            new Product { Id = 7, Name = "Trà thảo mộc detox", Slug = "tra-thao-moc-detox", Price = 175_000m, CompareAtPrice = 210_000m, CategoryId = 1, Badge = ProductBadge.Sale },
            new Product { Id = 8, Name = "Trà atiso Đà Lạt", Slug = "tra-atiso-da-lat", Price = 205_000m, CategoryId = 1 },
            new Product { Id = 9, Name = "Trà oolong túi lọc cao cấp", Slug = "tra-oolong-tui-loc-cao-cap", Price = 260_000m, CategoryId = 1 },
            new Product { Id = 10, Name = "Trà bạc hà hộp thiếc", Slug = "tra-bac-ha-hop-thiec", Price = 165_000m, CategoryId = 1 },

            // These two were renamed in place by the SeedTetProductsFromFigma migration. The
            // seed had kept the old names, so the model snapshot and the real database
            // disagreed — EF would have "helpfully" renamed them back on the next migration.
            new Product { Id = 20, Name = "Thiên điểu lạc hồng", Slug = "thien-dieu-lac-hong", Price = 899_000m, CategoryId = 5, Badge = ProductBadge.New, IsFeatured = true },
            new Product { Id = 21, Name = "Tinh hoa bắc bộ", Slug = "tinh-hoa-bac-bo", Price = 899_000m, CategoryId = 5, IsFeatured = true },
        };

        modelBuilder.Entity<Product>().HasData(products);
    }

    private static void SeedProductVariants(ModelBuilder modelBuilder)
    {
        var variants = new[]
        {
            new ProductVariant { Id = 1, ProductId = 1, Name = "Hộp 4 túi / màu vàng", PriceModifier = 0m, StockQuantity = 50 },
            new ProductVariant { Id = 2, ProductId = 1, Name = "Hộp 4 túi / màu đỏ", PriceModifier = 0m, StockQuantity = 30 },
            new ProductVariant { Id = 3, ProductId = 1, Name = "Hộp 8 túi / màu vàng", PriceModifier = 80_000m, StockQuantity = 20 },

            new ProductVariant { Id = 4, ProductId = 3, Name = "Hộp gỗ nhỏ", PriceModifier = 0m, StockQuantity = 15 },
            new ProductVariant { Id = 5, ProductId = 3, Name = "Hộp gỗ lớn", PriceModifier = 120_000m, StockQuantity = 10 },
        };

        modelBuilder.Entity<ProductVariant>().HasData(variants);
    }

    /// <summary>
    /// Sample subset only (a handful of provinces/wards) for the address form's cascading
    /// dropdowns, using the post-2025-reform 2-tier structure (Tỉnh/Thành phố → Phường/Xã).
    /// A full official dataset import is required before production use.
    /// </summary>
    private static void SeedGeo(ModelBuilder modelBuilder)
    {
        var provinces = new[]
        {
            new Province { Id = 1, Name = "Hà Nội" },
            new Province { Id = 2, Name = "TP. Hồ Chí Minh" },
            new Province { Id = 3, Name = "Đà Nẵng" },
            new Province { Id = 4, Name = "Hải Phòng" },
            new Province { Id = 5, Name = "Cần Thơ" },
        };
        modelBuilder.Entity<Province>().HasData(provinces);

        var wards = new[]
        {
            new Ward { Id = 1, ProvinceId = 1, Name = "Phường Việt Hưng" },
            new Ward { Id = 2, ProvinceId = 1, Name = "Phường Hoàn Kiếm" },
            new Ward { Id = 3, ProvinceId = 1, Name = "Phường Cầu Giấy" },

            new Ward { Id = 4, ProvinceId = 2, Name = "Phường Bến Thành" },
            new Ward { Id = 5, ProvinceId = 2, Name = "Phường Thủ Đức" },

            new Ward { Id = 6, ProvinceId = 3, Name = "Phường Hải Châu" },
            new Ward { Id = 7, ProvinceId = 3, Name = "Phường Thanh Khê" },

            new Ward { Id = 8, ProvinceId = 4, Name = "Phường Hồng Bàng" },
            new Ward { Id = 9, ProvinceId = 4, Name = "Phường Lê Chân" },

            new Ward { Id = 10, ProvinceId = 5, Name = "Phường Ninh Kiều" },
            new Ward { Id = 11, ProvinceId = 5, Name = "Phường Cái Răng" },
        };
        modelBuilder.Entity<Ward>().HasData(wards);
    }
}
