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
    }
}
