using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Context;

public class PickleChicDbContext : DbContext
{
    public PickleChicDbContext()
    {
    }

    public PickleChicDbContext(DbContextOptions<PickleChicDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Rank> Ranks { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Staff> Staff { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<ProductVariantImage> ProductVariantImages { get; set; }
    public DbSet<ProductAttribute> ProductAttributes { get; set; }
    public DbSet<AttributeValue> AttributeValues { get; set; }
    public DbSet<ProductVariantAttribute> ProductVariantAttributes { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<PointHistory> PointHistories { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<PromotionDetail> PromotionDetails { get; set; }
    public DbSet<Voucher> Vouchers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                "Data Source=localhost;Initial Catalog=PickleChic;TrustServerCertificate=True;User Id=sa; Password=123456");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.ShippingAddress)
            .WithMany(a => a.Orders)
            .HasForeignKey(o => o.ShippingAddressId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.PaymentMethod)
            .WithMany(p => p.Orders)
            .HasForeignKey(o => o.PaymentMethodId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PointHistory>()
            .HasOne(p => p.Customer)
            .WithMany(c => c.PointHistories)
            .HasForeignKey(p => p.CustomerId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.ProductVariant)
            .WithMany(pv => pv.OrderItems)
            .HasForeignKey(oi => oi.ProductVariantId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Rank>().HasData(
            new Rank { Id = 1, RankName = "Đồng", MinPoints = 0 },
            new Rank { Id = 2, RankName = "Bạc", MinPoints = 100 },
            new Rank { Id = 3, RankName = "Vàng", MinPoints = 500 }
        );

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, RoleName = "Admin", Status = 1, Permissions = "Waiting for list permissions" },
            new Role { Id = 2, RoleName = "Customer", Status = 1, Permissions = "Waiting for list permissions" }
        );

        modelBuilder.Entity<Staff>().HasData(
            new Staff
            {
                Id = 1,
                FullName = "Administrator",
                UserName = "admin",
                Email = "admin@example.com",
                PhoneNumber = "0123456789",
                PasswordHash = "C750DEC2A8526D8F49DD768D095F54D3",
                RoleId = 1,
                Status = 1
            }
        );

        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                Id = 1,
                Username = "customer",
                FullName = "Customer",
                Email = "customer@example.com",
                PasswordHash = "4E7282BE3B013E7C38590F6483C366EF",
                PhoneNumber = "0987654321",
                Gender = true,
                DateOfBirth = new DateTime(2000, 1, 1),
                TotalPoints = 0,
                Status = 1,
                RankId = 1
            }
        );

        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = 1,
                Name = "Vợt Pickleball",
                LinkImage = "https://example.com/images/racket-category.jpg",
                Description = "Các dòng vợt pickleball chính hãng cao cấp dành cho mọi trình độ",
                Status = 1,
                InsertedAt = new DateTime(2026, 6, 4, 12, 0, 0),
                Delete = false
            }
        );

        modelBuilder.Entity<Brand>().HasData(
            new Brand
            {
                Id = 1,
                Name = "PickleChic",
                Description = "Thương hiệu Pickleball phong cách, thời thượng hàng đầu cho phái đẹp",
                Status = 1,
                Delete = false,
                InsertedAt = new DateTime(2026, 6, 4, 12, 0, 0)
            }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                ProductName = "Vợt PickleChic Pro Carbon",
                Description = "Vợt Pickleball làm từ sợi carbon T700 cao cấp siêu nhẹ, thiết kế sang trọng thanh lịch.",
                CategoryId = 1,
                BrandId = 1,
                Status = 1,
                CreatedAt = new DateTime(2026, 6, 4, 12, 0, 0),
                IsDeleted = false
            }
        );

        modelBuilder.Entity<ProductAttribute>().HasData(
            new ProductAttribute { Id = 1, AttributeName = "Kích thước" },
            new ProductAttribute { Id = 2, AttributeName = "Màu sắc" }
        );

        modelBuilder.Entity<AttributeValue>().HasData(
            new AttributeValue { Id = 1, AttributeId = 1, Value = "Tiêu chuẩn", Note = "Độ dày tiêu chuẩn 16mm" },
            new AttributeValue { Id = 2, AttributeId = 2, Value = "Hồng Pastel", Note = "Màu hồng nữ tính, ngọt ngào" },
            new AttributeValue { Id = 3, AttributeId = 2, Value = "Trắng Chic", Note = "Màu trắng ngọc trai, sang trọng" }
        );

        modelBuilder.Entity<ProductVariant>().HasData(
            new ProductVariant
            {
                Id = 1,
                ProductId = 1,
                SKU = "PC-PRO-STD-PNK",
                VariantName = "Pro Carbon - Tiêu chuẩn - Hồng",
                Price = 1890000m,
                StockQuantity = 50,
                Status = 1
            },
            new ProductVariant
            {
                Id = 2,
                ProductId = 1,
                SKU = "PC-PRO-STD-WHT",
                VariantName = "Pro Carbon - Tiêu chuẩn - Trắng",
                Price = 1890000m,
                StockQuantity = 30,
                Status = 1
            }
        );

        modelBuilder.Entity<ProductVariantImage>().HasData(
            new ProductVariantImage
            {
                Id = 1,
                ProductVariantId = 1,
                URL = "https://example.com/images/pc-pro-pnk.jpg",
                Name = "Pro Carbon Hồng Mặt Trước",
                Description = "Màu hồng pastel tươi sáng",
                IsMain = true
            },
            new ProductVariantImage
            {
                Id = 2,
                ProductVariantId = 2,
                URL = "https://example.com/images/pc-pro-wht.jpg",
                Name = "Pro Carbon Trắng Mặt Trước",
                Description = "Màu trắng ngọc trai tinh tế",
                IsMain = true
            }
        );

        modelBuilder.Entity<ProductVariantAttribute>().HasData(
            new ProductVariantAttribute { Id = 1, ProductVariantId = 1, AttributeValueId = 1 },
            new ProductVariantAttribute { Id = 2, ProductVariantId = 1, AttributeValueId = 2 },
            new ProductVariantAttribute { Id = 3, ProductVariantId = 2, AttributeValueId = 1 },
            new ProductVariantAttribute { Id = 4, ProductVariantId = 2, AttributeValueId = 3 }
        );
    }
}
