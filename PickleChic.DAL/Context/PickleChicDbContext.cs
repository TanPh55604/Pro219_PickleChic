using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
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
    public DbSet<Province> Provinces { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<Ward> Wards { get; set; }
    public DbSet<Review> Reviews { get; set; }

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
            .HasOne(o => o.Address)
            .WithMany(a => a.Orders)
            .HasForeignKey(o => o.AddressId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasIndex(p => p.Code).IsUnique();
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasIndex(d => d.Code).IsUnique();
            entity.HasOne(d => d.Province)
                .WithMany(p => p.Districts)
                .HasForeignKey(d => d.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Ward>(entity =>
        {
            entity.HasIndex(w => w.Code).IsUnique();
            entity.HasOne(w => w.District)
                .WithMany(d => d.Wards)
                .HasForeignKey(w => w.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasOne(a => a.Ward)
                .WithMany(w => w.Addresses)
                .HasForeignKey(a => a.WardId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        try
        {
            var jsonPath = FindJsonPath();
            var jsonString = File.ReadAllText(jsonPath);
            var jsonProvinces = JsonSerializer.Deserialize<List<JsonProvince>>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (jsonProvinces != null)
            {
                var provinces = new List<Province>();
                var districts = new List<District>();
                var wards = new List<Ward>();

                int provinceId = 1;
                int districtId = 1;
                int wardId = 1;
                var now = new DateTime(2026, 6, 25, 12, 0, 0);

                foreach (var jp in jsonProvinces)
                {
                    var province = new Province
                    {
                        Id = provinceId++,
                        Name = jp.Name,
                        Code = jp.Code,
                        InsertedAt = now
                    };
                    provinces.Add(province);

                    foreach (var jd in jp.Districts)
                    {
                        var district = new District
                        {
                            Id = districtId++,
                            Name = jd.Name,
                            Code = jd.Code,
                            ProvinceId = province.Id,
                            InsertedAt = now
                        };
                        districts.Add(district);

                        foreach (var jw in jd.Wards)
                        {
                            var ward = new Ward
                            {
                                Id = wardId++,
                                Name = jw.Name,
                                Code = jw.Code,
                                DistrictId = district.Id,
                                InsertedAt = now
                            };
                            wards.Add(ward);
                        }
                    }
                }

                modelBuilder.Entity<Province>().HasData(provinces);
                modelBuilder.Entity<District>().HasData(districts);
                modelBuilder.Entity<Ward>().HasData(wards);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to seed locations data: {ex.Message}", ex);
        }

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

        modelBuilder.Entity<Review>()
            .HasOne(r => r.OrderItem)
            .WithOne(oi => oi.Review)
            .HasForeignKey<Review>(r => r.OrderItemId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.ProductVariant)
            .WithMany(pv => pv.Reviews)
            .HasForeignKey(r => r.ProductVariantId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Rank>().HasData(
            new Rank { Id = 1, RankName = "Đồng", SpendAmount = 0 },
            new Rank { Id = 2, RankName = "Bạc", SpendAmount = 1000000 },
            new Rank { Id = 3, RankName = "Vàng", SpendAmount = 5000000 }
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
                PasswordHash = "1b97db3e7bb476c2757d2d12f0bca777",
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
            },
            new Customer
            {
                Id = -1,
                Username = "guest",
                FullName = "guest",
                Email = "guest@example.com",
                PasswordHash = "4E7282BE3B013E7C38590F6483C366EF",
                PhoneNumber = "0000000000",
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

        modelBuilder.Entity<ProductAttribute>()
            .HasOne(a => a.Category)
            .WithMany()
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ProductAttribute>().HasData(
            new ProductAttribute { Id = 1, AttributeName = "Kích thước", CategoryId = 1 },
            new ProductAttribute { Id = 2, AttributeName = "Màu sắc", CategoryId = 1 }
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

        modelBuilder.Entity<PaymentMethod>().HasData(
            new PaymentMethod
            {
                Id = 1,
                Name = "Thanh toán tiền mặt khi nhận hàng",
                Description = "Thanh toán bằng tiền mặt khi nhận hàng",
                InsertedAt = new DateTime(2026, 6, 25, 12, 0, 0),
                Delete = false
            },
            new PaymentMethod
            {
                Id = 2,
                Name = "Chuyển khoản",
                Description = "Thanh toán chuyển khoản qua tài khoản ngân hàng",
                InsertedAt = new DateTime(2026, 6, 25, 12, 0, 0),
                Delete = false
            }
        );
    }

    private static string FindJsonPath()
    {
        var assemblyDir = Path.GetDirectoryName(typeof(PickleChicDbContext).Assembly.Location);
        if (!string.IsNullOrEmpty(assemblyDir))
        {
            var path = Path.Combine(assemblyDir, "Context", "locations.json");
            if (File.Exists(path)) return path;

            path = Path.Combine(assemblyDir, "locations.json");
            if (File.Exists(path)) return path;
        }

        var currentDir = Directory.GetCurrentDirectory();
        while (!string.IsNullOrEmpty(currentDir))
        {
            var path1 = Path.Combine(currentDir, "PickleChic.DAL", "Context", "locations.json");
            if (File.Exists(path1)) return path1;

            var path2 = Path.Combine(currentDir, "Context", "locations.json");
            if (File.Exists(path2)) return path2;

            var path3 = Path.Combine(currentDir, "locations.json");
            if (File.Exists(path3)) return path3;

            var parent = Directory.GetParent(currentDir);
            if (parent == null || parent.FullName == currentDir) break;
            currentDir = parent.FullName;
        }

        throw new FileNotFoundException("Could not find locations.json");
    }

    private class JsonProvince
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public List<JsonDistrict> Districts { get; set; } = new();
    }

    private class JsonDistrict
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public List<JsonWard> Wards { get; set; } = new();
    }

    private class JsonWard
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
