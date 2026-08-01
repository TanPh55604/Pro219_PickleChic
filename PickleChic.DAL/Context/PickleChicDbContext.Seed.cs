using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Context;

public partial class PickleChicDbContext
{
    private static readonly DateTime SeedAt = new(2026, 6, 4, 12, 0, 0);

    private static void SeedExampleData(ModelBuilder modelBuilder)
    {
        SeedCatalog(modelBuilder);
        SeedAddresses(modelBuilder);
        SeedPaymentMethods(modelBuilder);
        SeedVouchers(modelBuilder);
        SeedPosOrdersAndReviews(modelBuilder);
    }

    private static void SeedCatalog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = 1,
                Name = "Vợt",
                LinkImage = "/uploads/categories/1/vot.webp",
                Description = "Vợt pickleball chính hãng cho mọi trình độ",
                Status = 1,
                InsertedAt = SeedAt,
                Delete = false
            },
            new Category
            {
                Id = 2,
                Name = "Bóng",
                LinkImage = "/uploads/categories/2/bong.webp",
                Description = "Bóng pickleball indoor / outdoor đạt chuẩn thi đấu",
                Status = 1,
                InsertedAt = SeedAt,
                Delete = false
            },
            new Category
            {
                Id = 3,
                Name = "Quần",
                LinkImage = "/uploads/categories/3/quan.webp",
                Description = "Quần short thể thao pickleball thoáng mát, co giãn",
                Status = 1,
                InsertedAt = SeedAt,
                Delete = false
            }
        );

        modelBuilder.Entity<Brand>().HasData(
            new Brand
            {
                Id = 1,
                Name = "Selkirk",
                Description = "Thương hiệu vợt pickleball hàng đầu Mỹ, nổi bật với công nghệ carbon và kiểm soát bóng.",
                Status = 1,
                Delete = false,
                InsertedAt = SeedAt
            },
            new Brand
            {
                Id = 2,
                Name = "JOOLA",
                Description = "Thương hiệu pickleball / table tennis quốc tế, đối tác nhiều VĐV chuyên nghiệp.",
                Status = 1,
                Delete = false,
                InsertedAt = SeedAt
            },
            new Brand
            {
                Id = 3,
                Name = "Franklin",
                Description = "Franklin Sports — bóng và phụ kiện pickleball phổ biến trên toàn cầu.",
                Status = 1,
                Delete = false,
                InsertedAt = SeedAt
            }
        );

        // Attributes by category
        // Vợt: Độ dày, Màu sắc
        // Bóng: Loại sân, Màu sắc
        // Quần: Size, Màu sắc
        modelBuilder.Entity<ProductAttribute>().HasData(
            new ProductAttribute { Id = 1, AttributeName = "Độ dày", CategoryId = 1 },
            new ProductAttribute { Id = 2, AttributeName = "Màu sắc", CategoryId = 1 },
            new ProductAttribute { Id = 3, AttributeName = "Loại sân", CategoryId = 2 },
            new ProductAttribute { Id = 4, AttributeName = "Màu sắc", CategoryId = 2 },
            new ProductAttribute { Id = 5, AttributeName = "Size", CategoryId = 3 },
            new ProductAttribute { Id = 6, AttributeName = "Màu sắc", CategoryId = 3 }
        );

        modelBuilder.Entity<AttributeValue>().HasData(
            // Vợt - Độ dày
            new AttributeValue { Id = 1, AttributeId = 1, Value = "14mm", Note = "Phản hồi nhanh, phù hợp tấn công" },
            new AttributeValue { Id = 2, AttributeId = 1, Value = "16mm", Note = "Kiểm soát tốt, phổ biến thi đấu" },
            // Vợt - Màu
            new AttributeValue { Id = 3, AttributeId = 2, Value = "Hồng", Note = null },
            new AttributeValue { Id = 4, AttributeId = 2, Value = "Đen", Note = null },
            new AttributeValue { Id = 5, AttributeId = 2, Value = "Trắng", Note = null },
            new AttributeValue { Id = 6, AttributeId = 2, Value = "Xanh navy", Note = null },
            // Bóng
            new AttributeValue { Id = 7, AttributeId = 3, Value = "Outdoor", Note = "40 lỗ, dùng ngoài trời" },
            new AttributeValue { Id = 8, AttributeId = 4, Value = "Vàng", Note = "Màu chuẩn thi đấu outdoor" },
            // Quần - Size
            new AttributeValue { Id = 9, AttributeId = 5, Value = "S", Note = null },
            new AttributeValue { Id = 10, AttributeId = 5, Value = "M", Note = null },
            new AttributeValue { Id = 11, AttributeId = 5, Value = "L", Note = null },
            new AttributeValue { Id = 12, AttributeId = 5, Value = "XL", Note = null },
            // Quần - Màu
            new AttributeValue { Id = 13, AttributeId = 6, Value = "Đen", Note = null },
            new AttributeValue { Id = 14, AttributeId = 6, Value = "Hồng", Note = null },
            new AttributeValue { Id = 15, AttributeId = 6, Value = "Trắng", Note = null }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                ProductName = "Selkirk Luxx Control Air",
                Description = "Vợt kiểm soát cao cấp từ Selkirk, mặt carbon T700, cảm giác mềm và ổn định.",
                CategoryId = 1,
                BrandId = 1,
                Status = 1,
                CreatedAt = SeedAt,
                IsDeleted = false
            },
            new Product
            {
                Id = 2,
                ProductName = "JOOLA Perseus 16",
                Description = "Vợt JOOLA Perseus thế hệ mới, công nghệ Hyperfoam, cân bằng lực và kiểm soát.",
                CategoryId = 1,
                BrandId = 2,
                Status = 1,
                CreatedAt = SeedAt,
                IsDeleted = false
            },
            new Product
            {
                Id = 3,
                ProductName = "Franklin X-40 Outdoor",
                Description = "Bóng outdoor tiêu chuẩn USAPA, độ bền cao, bay ổn định ngoài trời.",
                CategoryId = 2,
                BrandId = 3,
                Status = 1,
                CreatedAt = SeedAt,
                IsDeleted = false
            },
            new Product
            {
                Id = 4,
                ProductName = "JOOLA Trinity Shorts",
                Description = "Quần short JOOLA nhẹ, thấm hút nhanh, phù hợp tập và thi đấu.",
                CategoryId = 3,
                BrandId = 2,
                Status = 1,
                CreatedAt = SeedAt,
                IsDeleted = false
            },
            new Product
            {
                Id = 5,
                ProductName = "Selkirk Pro Line Shorts",
                Description = "Quần short Selkirk Pro Line co giãn 4 chiều, thoải mái di chuyển trên sân.",
                CategoryId = 3,
                BrandId = 1,
                Status = 1,
                CreatedAt = SeedAt,
                IsDeleted = false
            }
        );

        modelBuilder.Entity<ProductVariant>().HasData(
            // Product 1 - Selkirk vợt (4)
            new ProductVariant { Id = 1, ProductId = 1, SKU = "SEL-LUXX-14-PNK", VariantName = "Luxx Control Air - 14mm - Hồng", Price = 4590000m, StockQuantity = 20, Status = 1 },
            new ProductVariant { Id = 2, ProductId = 1, SKU = "SEL-LUXX-16-BLK", VariantName = "Luxx Control Air - 16mm - Đen", Price = 4790000m, StockQuantity = 25, Status = 1 },
            new ProductVariant { Id = 3, ProductId = 1, SKU = "SEL-LUXX-16-WHT", VariantName = "Luxx Control Air - 16mm - Trắng", Price = 4790000m, StockQuantity = 18, Status = 1 },
            new ProductVariant { Id = 4, ProductId = 1, SKU = "SEL-LUXX-14-NVY", VariantName = "Luxx Control Air - 14mm - Xanh navy", Price = 4590000m, StockQuantity = 15, Status = 1 },
            // Product 2 - JOOLA vợt (4)
            new ProductVariant { Id = 5, ProductId = 2, SKU = "JOL-PER-16-BLK", VariantName = "Perseus 16 - 16mm - Đen", Price = 5290000m, StockQuantity = 22, Status = 1 },
            new ProductVariant { Id = 6, ProductId = 2, SKU = "JOL-PER-16-PNK", VariantName = "Perseus 16 - 16mm - Hồng", Price = 5290000m, StockQuantity = 16, Status = 1 },
            new ProductVariant { Id = 7, ProductId = 2, SKU = "JOL-PER-14-WHT", VariantName = "Perseus 16 - 14mm - Trắng", Price = 5090000m, StockQuantity = 14, Status = 1 },
            new ProductVariant { Id = 8, ProductId = 2, SKU = "JOL-PER-16-NVY", VariantName = "Perseus 16 - 16mm - Xanh navy", Price = 5290000m, StockQuantity = 12, Status = 1 },
            // Product 3 - Franklin bóng (1)
            new ProductVariant { Id = 9, ProductId = 3, SKU = "FRA-X40-OUT-YLW", VariantName = "X-40 Outdoor - Vàng", Price = 89000m, StockQuantity = 200, Status = 1 },
            // Product 4 - JOOLA quần (3)
            new ProductVariant { Id = 10, ProductId = 4, SKU = "JOL-TRI-S-BLK", VariantName = "Trinity Shorts - S - Đen", Price = 790000m, StockQuantity = 30, Status = 1 },
            new ProductVariant { Id = 11, ProductId = 4, SKU = "JOL-TRI-M-PNK", VariantName = "Trinity Shorts - M - Hồng", Price = 790000m, StockQuantity = 28, Status = 1 },
            new ProductVariant { Id = 12, ProductId = 4, SKU = "JOL-TRI-L-WHT", VariantName = "Trinity Shorts - L - Trắng", Price = 790000m, StockQuantity = 24, Status = 1 },
            // Product 5 - Selkirk quần (2)
            new ProductVariant { Id = 13, ProductId = 5, SKU = "SEL-PRO-M-BLK", VariantName = "Pro Line Shorts - M - Đen", Price = 890000m, StockQuantity = 20, Status = 1 },
            new ProductVariant { Id = 14, ProductId = 5, SKU = "SEL-PRO-L-PNK", VariantName = "Pro Line Shorts - L - Hồng", Price = 890000m, StockQuantity = 18, Status = 1 }
        );

        modelBuilder.Entity<ProductVariantImage>().HasData(
            new ProductVariantImage { Id = 1, ProductVariantId = 1, URL = "/uploads/products/1/sel-luxx-pink.jpg", Name = "Luxx Hồng", Description = null, IsMain = true },
            new ProductVariantImage { Id = 2, ProductVariantId = 2, URL = "/uploads/products/1/sel-luxx-black.jpg", Name = "Luxx Đen", Description = null, IsMain = true },
            new ProductVariantImage { Id = 3, ProductVariantId = 3, URL = "/uploads/products/1/sel-luxx-white.jpg", Name = "Luxx Trắng", Description = null, IsMain = true },
            new ProductVariantImage { Id = 4, ProductVariantId = 4, URL = "/uploads/products/1/sel-luxx-navy.jpg", Name = "Luxx Navy", Description = null, IsMain = true },
            new ProductVariantImage { Id = 5, ProductVariantId = 5, URL = "/uploads/products/2/jol-perseus-black.jpg", Name = "Perseus Đen", Description = null, IsMain = true },
            new ProductVariantImage { Id = 6, ProductVariantId = 6, URL = "/uploads/products/2/jol-perseus-pink.jpg", Name = "Perseus Hồng", Description = null, IsMain = true },
            new ProductVariantImage { Id = 7, ProductVariantId = 7, URL = "/uploads/products/2/jol-perseus-white.jpg", Name = "Perseus Trắng", Description = null, IsMain = true },
            new ProductVariantImage { Id = 8, ProductVariantId = 8, URL = "/uploads/products/2/jol-perseus-navy.jpg", Name = "Perseus Navy", Description = null, IsMain = true },
            new ProductVariantImage { Id = 9, ProductVariantId = 9, URL = "/uploads/products/3/franklin-x40.jpg", Name = "X-40 Vàng", Description = null, IsMain = true },
            new ProductVariantImage { Id = 10, ProductVariantId = 10, URL = "/uploads/products/4/jol-trinity-s-black.jpg", Name = "Trinity S Đen", Description = null, IsMain = true },
            new ProductVariantImage { Id = 11, ProductVariantId = 11, URL = "/uploads/products/4/jol-trinity-m-pink.jpg", Name = "Trinity M Hồng", Description = null, IsMain = true },
            new ProductVariantImage { Id = 12, ProductVariantId = 12, URL = "/uploads/products/4/jol-trinity-l-white.jpg", Name = "Trinity L Trắng", Description = null, IsMain = true },
            new ProductVariantImage { Id = 13, ProductVariantId = 13, URL = "/uploads/products/5/sel-pro-m-black.jpg", Name = "Pro Line M Đen", Description = null, IsMain = true },
            new ProductVariantImage { Id = 14, ProductVariantId = 14, URL = "/uploads/products/5/sel-pro-l-pink.jpg", Name = "Pro Line L Hồng", Description = null, IsMain = true }
        );

        modelBuilder.Entity<ProductVariantAttribute>().HasData(
            // P1 variants: thickness + color
            new ProductVariantAttribute { Id = 1, ProductVariantId = 1, AttributeValueId = 1 },
            new ProductVariantAttribute { Id = 2, ProductVariantId = 1, AttributeValueId = 3 },
            new ProductVariantAttribute { Id = 3, ProductVariantId = 2, AttributeValueId = 2 },
            new ProductVariantAttribute { Id = 4, ProductVariantId = 2, AttributeValueId = 4 },
            new ProductVariantAttribute { Id = 5, ProductVariantId = 3, AttributeValueId = 2 },
            new ProductVariantAttribute { Id = 6, ProductVariantId = 3, AttributeValueId = 5 },
            new ProductVariantAttribute { Id = 7, ProductVariantId = 4, AttributeValueId = 1 },
            new ProductVariantAttribute { Id = 8, ProductVariantId = 4, AttributeValueId = 6 },
            // P2
            new ProductVariantAttribute { Id = 9, ProductVariantId = 5, AttributeValueId = 2 },
            new ProductVariantAttribute { Id = 10, ProductVariantId = 5, AttributeValueId = 4 },
            new ProductVariantAttribute { Id = 11, ProductVariantId = 6, AttributeValueId = 2 },
            new ProductVariantAttribute { Id = 12, ProductVariantId = 6, AttributeValueId = 3 },
            new ProductVariantAttribute { Id = 13, ProductVariantId = 7, AttributeValueId = 1 },
            new ProductVariantAttribute { Id = 14, ProductVariantId = 7, AttributeValueId = 5 },
            new ProductVariantAttribute { Id = 15, ProductVariantId = 8, AttributeValueId = 2 },
            new ProductVariantAttribute { Id = 16, ProductVariantId = 8, AttributeValueId = 6 },
            // P3 bóng
            new ProductVariantAttribute { Id = 17, ProductVariantId = 9, AttributeValueId = 7 },
            new ProductVariantAttribute { Id = 18, ProductVariantId = 9, AttributeValueId = 8 },
            // P4 quần
            new ProductVariantAttribute { Id = 19, ProductVariantId = 10, AttributeValueId = 9 },
            new ProductVariantAttribute { Id = 20, ProductVariantId = 10, AttributeValueId = 13 },
            new ProductVariantAttribute { Id = 21, ProductVariantId = 11, AttributeValueId = 10 },
            new ProductVariantAttribute { Id = 22, ProductVariantId = 11, AttributeValueId = 14 },
            new ProductVariantAttribute { Id = 23, ProductVariantId = 12, AttributeValueId = 11 },
            new ProductVariantAttribute { Id = 24, ProductVariantId = 12, AttributeValueId = 15 },
            // P5 quần
            new ProductVariantAttribute { Id = 25, ProductVariantId = 13, AttributeValueId = 10 },
            new ProductVariantAttribute { Id = 26, ProductVariantId = 13, AttributeValueId = 13 },
            new ProductVariantAttribute { Id = 27, ProductVariantId = 14, AttributeValueId = 11 },
            new ProductVariantAttribute { Id = 28, ProductVariantId = 14, AttributeValueId = 14 }
        );
    }

    private static void SeedAddresses(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>().HasData(
            new Address
            {
                Id = 1,
                CustomerId = 1,
                FullName = "Nhận tại quầy",
                PhoneNumber = "0987654321",
                DetailInfo = "Mua tại quầy",
                WardId = 1,
                IsDefault = false,
                Status = 0,
                InsertedAt = SeedAt,
                Delete = false
            },
            new Address
            {
                Id = 2,
                CustomerId = -1,
                FullName = "Khách vãng lai",
                PhoneNumber = "0000000000",
                DetailInfo = "Mua tại quầy",
                WardId = 1,
                IsDefault = false,
                Status = 0,
                InsertedAt = SeedAt,
                Delete = false
            }
        );
    }

    private static void SeedPaymentMethods(ModelBuilder modelBuilder)
    {
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

    private static void SeedVouchers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Voucher>().HasData(
            // Expired
            new Voucher
            {
                Id = 1,
                VoucherCode = "WELCOME50K",
                DiscountType = "FixedAmount",
                DiscountValue = 50000m,
                MinOrderValue = 300000m,
                MaxDiscountAmount = 50000m,
                MinimumSpend = 300000m,
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 3, 31, 23, 59, 59),
                UsageLimit = 100,
                CustomerUsageLimit = 1,
                UsedCount = 42,
                IsActive = false
            },
            new Voucher
            {
                Id = 2,
                VoucherCode = "SPRING20",
                DiscountType = "Percentage",
                DiscountValue = 20m,
                MinOrderValue = 500000m,
                MaxDiscountAmount = 200000m,
                MinimumSpend = 500000m,
                StartDate = new DateTime(2026, 4, 1),
                EndDate = new DateTime(2026, 5, 31, 23, 59, 59),
                UsageLimit = 200,
                CustomerUsageLimit = 2,
                UsedCount = 88,
                IsActive = false
            },
            // Ongoing (relative to Aug 2026)
            new Voucher
            {
                Id = 3,
                VoucherCode = "SUMMER15",
                DiscountType = "Percentage",
                DiscountValue = 15m,
                MinOrderValue = 400000m,
                MaxDiscountAmount = 300000m,
                MinimumSpend = 400000m,
                StartDate = new DateTime(2026, 7, 1),
                EndDate = new DateTime(2026, 9, 30, 23, 59, 59),
                UsageLimit = 500,
                CustomerUsageLimit = 3,
                UsedCount = 37,
                IsActive = true
            },
            new Voucher
            {
                Id = 4,
                VoucherCode = "PICKLE100K",
                DiscountType = "FixedAmount",
                DiscountValue = 100000m,
                MinOrderValue = 800000m,
                MaxDiscountAmount = 100000m,
                MinimumSpend = 800000m,
                StartDate = new DateTime(2026, 7, 15),
                EndDate = new DateTime(2026, 10, 15, 23, 59, 59),
                UsageLimit = 300,
                CustomerUsageLimit = 1,
                UsedCount = 19,
                IsActive = true
            },
            // Future
            new Voucher
            {
                Id = 5,
                VoucherCode = "TET2027",
                DiscountType = "Percentage",
                DiscountValue = 25m,
                MinOrderValue = 1000000m,
                MaxDiscountAmount = 500000m,
                MinimumSpend = 1000000m,
                StartDate = new DateTime(2026, 12, 20),
                EndDate = new DateTime(2027, 2, 15, 23, 59, 59),
                UsageLimit = 150,
                CustomerUsageLimit = 1,
                UsedCount = 0,
                IsActive = false
            },
            new Voucher
            {
                Id = 6,
                VoucherCode = "NEWYEAR50K",
                DiscountType = "FixedAmount",
                DiscountValue = 50000m,
                MinOrderValue = 350000m,
                MaxDiscountAmount = 50000m,
                MinimumSpend = 350000m,
                StartDate = new DateTime(2027, 1, 1),
                EndDate = new DateTime(2027, 3, 31, 23, 59, 59),
                UsageLimit = 400,
                CustomerUsageLimit = 2,
                UsedCount = 0,
                IsActive = false
            }
        );
    }

    private static void SeedPosOrdersAndReviews(ModelBuilder modelBuilder)
    {
        const string doneHistory =
            "[{\"index\":1,\"status\":\"Hoàn thành\",\"orderStatus\":\"Hoàn thành\",\"paymentStatus\":\"Đã thanh toán\",\"dateTime\":\"10:30 20/07/2026\"}]";

        modelBuilder.Entity<Order>().HasData(
            new Order
            {
                Id = 1,
                CustomerId = 1,
                OrderCode = "DH100001",
                AddressId = 1,
                OrderDate = new DateTime(2026, 7, 10, 10, 15, 0),
                PaymentMethodId = 1,
                VoucherId = null,
                PaymentStatus = "Đã thanh toán",
                OrderStatus = "Hoàn thành",
                Notes = "Mua tại quầy",
                LastUpdate = new DateTime(2026, 7, 10, 10, 20, 0),
                Delete = false,
                CustomerType = "Registered",
                IsOrderPOS = true,
                BOPIS = false,
                ShippingFee = 0m,
                StatusHistory = doneHistory,
                UpdateBy = "admin",
                Status = 7,
                InsertedAt = new DateTime(2026, 7, 10, 10, 15, 0)
            },
            new Order
            {
                Id = 2,
                CustomerId = 1,
                OrderCode = "DH100002",
                AddressId = 1,
                OrderDate = new DateTime(2026, 7, 18, 15, 40, 0),
                PaymentMethodId = 1,
                VoucherId = null,
                PaymentStatus = "Đã thanh toán",
                OrderStatus = "Hoàn thành",
                Notes = "Mua tại quầy",
                LastUpdate = new DateTime(2026, 7, 18, 15, 45, 0),
                Delete = false,
                CustomerType = "Registered",
                IsOrderPOS = true,
                BOPIS = false,
                ShippingFee = 0m,
                StatusHistory = "[{\"index\":1,\"status\":\"Hoàn thành\",\"orderStatus\":\"Hoàn thành\",\"paymentStatus\":\"Đã thanh toán\",\"dateTime\":\"15:45 18/07/2026\"}]",
                UpdateBy = "admin",
                Status = 7,
                InsertedAt = new DateTime(2026, 7, 18, 15, 40, 0)
            },
            new Order
            {
                Id = 3,
                CustomerId = 1,
                OrderCode = "DH100003",
                AddressId = 1,
                OrderDate = new DateTime(2026, 7, 25, 11, 5, 0),
                PaymentMethodId = 1,
                VoucherId = null,
                PaymentStatus = "Đã thanh toán",
                OrderStatus = "Hoàn thành",
                Notes = "Mua tại quầy",
                LastUpdate = new DateTime(2026, 7, 25, 11, 10, 0),
                Delete = false,
                CustomerType = "Registered",
                IsOrderPOS = true,
                BOPIS = false,
                ShippingFee = 0m,
                StatusHistory = "[{\"index\":1,\"status\":\"Hoàn thành\",\"orderStatus\":\"Hoàn thành\",\"paymentStatus\":\"Đã thanh toán\",\"dateTime\":\"11:10 25/07/2026\"}]",
                UpdateBy = "admin",
                Status = 7,
                InsertedAt = new DateTime(2026, 7, 25, 11, 5, 0)
            }
        );

        // Order items — enough unique rows for 3–10 reviews per product
        // P1 (5), P2 (4), P3 (3), P4 (4), P5 (3) = 19 items
        modelBuilder.Entity<OrderItem>().HasData(
            // Order 1 — P1 x3, P3 x1, P4 x1
            new OrderItem { Id = 1, OrderId = 1, ProductVariantId = 1, PromotionId = null, Quantity = 1, UnitPrice = 4590000m, DiscountAmount = 0m, Subtotal = 4590000m, IsReviewed = true, InsertedAt = new DateTime(2026, 7, 10, 10, 15, 0), Delete = false },
            new OrderItem { Id = 2, OrderId = 1, ProductVariantId = 2, PromotionId = null, Quantity = 1, UnitPrice = 4790000m, DiscountAmount = 0m, Subtotal = 4790000m, IsReviewed = true, InsertedAt = new DateTime(2026, 7, 10, 10, 15, 0), Delete = false },
            new OrderItem { Id = 3, OrderId = 1, ProductVariantId = 3, PromotionId = null, Quantity = 1, UnitPrice = 4790000m, DiscountAmount = 0m, Subtotal = 4790000m, IsReviewed = true, InsertedAt = new DateTime(2026, 7, 10, 10, 15, 0), Delete = false },
            new OrderItem { Id = 4, OrderId = 1, ProductVariantId = 9, PromotionId = null, Quantity = 3, UnitPrice = 89000m, DiscountAmount = 0m, Subtotal = 267000m, IsReviewed = true, InsertedAt = new DateTime(2026, 7, 10, 10, 15, 0), Delete = false },
            new OrderItem { Id = 5, OrderId = 1, ProductVariantId = 10, PromotionId = null, Quantity = 1, UnitPrice = 790000m, DiscountAmount = 0m, Subtotal = 790000m, IsReviewed = true, InsertedAt = new DateTime(2026, 7, 10, 10, 15, 0), Delete = false },
            // Order 2 — P1 x2, P2 x2, P4 x2, P5 x1
            new OrderItem { Id = 6, OrderId = 2, ProductVariantId = 4, PromotionId = null, Quantity = 1, UnitPrice = 4590000m, DiscountAmount = 0m, Subtotal = 4590000m, IsReviewed = true, InsertedAt = new DateTime(2026, 7, 18, 15, 40, 0), Delete = false },
            new OrderItem { Id = 7, OrderId = 2, ProductVariantId = 1, PromotionId = null, Quantity = 1, UnitPrice = 4590000m, DiscountAmount = 0m, Subtotal = 4590000m, IsReviewed = true, InsertedAt = new DateTime(2026, 7, 18, 15, 40, 0), Delete = false },
            new OrderItem { Id = 8, OrderId = 2, ProductVariantId = 5, PromotionId = null, Quantity = 1, UnitPrice = 5290000m, DiscountAmount = 0m, Subtotal = 5290000m, IsReviewed = true, InsertedAt = new DateTime(2026, 7, 18, 15, 40, 0), Delete = false },
            new OrderItem { Id = 9, OrderId = 2, ProductVariantId = 6, PromotionId = null, Quantity = 1, UnitPrice = 5290000m, DiscountAmount = 0m, Subtotal = 5290000m, IsReviewed = true, InsertedAt = new DateTime(2026, 7, 18, 15, 40, 0), Delete = false },
            new OrderItem { Id = 10, OrderId = 2, ProductVariantId = 11, PromotionId = null, Quantity = 1, UnitPrice = 790000m, DiscountAmount = 0m, Subtotal = 790000m, IsReviewed = true, InsertedAt = new DateTime(2026, 7, 18, 15, 40, 0), Delete = false },
            new OrderItem { Id = 11, OrderId = 2, ProductVariantId = 12, PromotionId = null, Quantity = 1, UnitPrice = 790000m, DiscountAmount = 0m, Subtotal = 790000m, IsReviewed = true, InsertedAt = new DateTime(2026, 7, 18, 15, 40, 0), Delete = false },
            new OrderItem { Id = 12, OrderId = 2, ProductVariantId = 13, PromotionId = null, Quantity = 1, UnitPrice = 890000m, DiscountAmount = 0m, Subtotal = 890000m, IsReviewed = true, InsertedAt = new DateTime(2026, 7, 18, 15, 40, 0), Delete = false },
            // Order 3 — P2 x2, P3 x2, P4 x1, P5 x2
            new OrderItem { Id = 13, OrderId = 3, ProductVariantId = 7, PromotionId = null, Quantity = 1, UnitPrice = 5090000m, DiscountAmount = 0m, Subtotal = 5090000m, IsReviewed = true, InsertedAt = new DateTime(2026, 7, 25, 11, 5, 0), Delete = false },
            new OrderItem { Id = 14, OrderId = 3, ProductVariantId = 8, PromotionId = null, Quantity = 1, UnitPrice = 5290000m, DiscountAmount = 0m, Subtotal = 5290000m, IsReviewed = true, InsertedAt = new DateTime(2026, 7, 25, 11, 5, 0), Delete = false },
            new OrderItem { Id = 15, OrderId = 3, ProductVariantId = 9, PromotionId = null, Quantity = 6, UnitPrice = 89000m, DiscountAmount = 0m, Subtotal = 534000m, IsReviewed = true, InsertedAt = new DateTime(2026, 7, 25, 11, 5, 0), Delete = false },
            new OrderItem { Id = 16, OrderId = 3, ProductVariantId = 9, PromotionId = null, Quantity = 2, UnitPrice = 89000m, DiscountAmount = 0m, Subtotal = 178000m, IsReviewed = true, InsertedAt = new DateTime(2026, 7, 25, 11, 5, 0), Delete = false },
            new OrderItem { Id = 17, OrderId = 3, ProductVariantId = 10, PromotionId = null, Quantity = 1, UnitPrice = 790000m, DiscountAmount = 0m, Subtotal = 790000m, IsReviewed = true, InsertedAt = new DateTime(2026, 7, 25, 11, 5, 0), Delete = false },
            new OrderItem { Id = 18, OrderId = 3, ProductVariantId = 14, PromotionId = null, Quantity = 1, UnitPrice = 890000m, DiscountAmount = 0m, Subtotal = 890000m, IsReviewed = true, InsertedAt = new DateTime(2026, 7, 25, 11, 5, 0), Delete = false },
            new OrderItem { Id = 19, OrderId = 3, ProductVariantId = 13, PromotionId = null, Quantity = 1, UnitPrice = 890000m, DiscountAmount = 0m, Subtotal = 890000m, IsReviewed = true, InsertedAt = new DateTime(2026, 7, 25, 11, 5, 0), Delete = false }
        );

        modelBuilder.Entity<Review>().HasData(
            // Product 1 — 5 reviews (items 1,2,3,6,7)
            new Review { Id = 1, OrderItemId = 1, ProductVariantId = 1, Title = "Rất ưng", Content = "Vợt nhẹ, cảm giác bóng êm, kiểm soát tốt khi dink.", Overall = 5, Status = 1, CreateAt = new DateTime(2026, 7, 12, 9, 0, 0), Delete = false },
            new Review { Id = 2, OrderItemId = 2, ProductVariantId = 2, Title = "Đáng tiền", Content = "16mm ổn định, phù hợp người chơi trung cấp.", Overall = 5, Status = 1, CreateAt = new DateTime(2026, 7, 13, 14, 0, 0), Delete = false },
            new Review { Id = 3, OrderItemId = 3, ProductVariantId = 3, Title = "Màu đẹp", Content = "Màu trắng sang, cầm chắc tay.", Overall = 4, Status = 1, CreateAt = new DateTime(2026, 7, 14, 8, 30, 0), Delete = false },
            new Review { Id = 4, OrderItemId = 6, ProductVariantId = 4, Title = "Ok", Content = "Power đủ dùng, sweet spot rộng.", Overall = 4, Status = 1, CreateAt = new DateTime(2026, 7, 20, 10, 0, 0), Delete = false },
            new Review { Id = 5, OrderItemId = 7, ProductVariantId = 1, Title = "Mua lần 2", Content = "Vẫn thích bản hồng 14mm, phản hồi nhanh.", Overall = 5, Status = 1, CreateAt = new DateTime(2026, 7, 21, 16, 0, 0), Delete = false },
            // Product 2 — 4 reviews (items 8,9,13,14)
            new Review { Id = 6, OrderItemId = 8, ProductVariantId = 5, Title = "Chuẩn pro", Content = "Perseus đánh drive rất đã, độ nảy ổn.", Overall = 5, Status = 1, CreateAt = new DateTime(2026, 7, 20, 11, 0, 0), Delete = false },
            new Review { Id = 7, OrderItemId = 9, ProductVariantId = 6, Title = "Hồng xinh", Content = "Form đẹp, balance tốt.", Overall = 4, Status = 1, CreateAt = new DateTime(2026, 7, 21, 9, 0, 0), Delete = false },
            new Review { Id = 8, OrderItemId = 13, ProductVariantId = 7, Title = "14mm nhanh", Content = "Phản hồi nhanh, hợp người thích tốc độ.", Overall = 5, Status = 1, CreateAt = new DateTime(2026, 7, 26, 12, 0, 0), Delete = false },
            new Review { Id = 9, OrderItemId = 14, ProductVariantId = 8, Title = "Tốt", Content = "Cảm giác premium, grip êm.", Overall = 4, Status = 1, CreateAt = new DateTime(2026, 7, 27, 8, 0, 0), Delete = false },
            // Product 3 — 3 reviews (items 4,15,16)
            new Review { Id = 10, OrderItemId = 4, ProductVariantId = 9, Title = "Bóng chuẩn", Content = "X-40 bay ổn, ít méo sau vài buổi chơi.", Overall = 5, Status = 1, CreateAt = new DateTime(2026, 7, 11, 18, 0, 0), Delete = false },
            new Review { Id = 11, OrderItemId = 15, ProductVariantId = 9, Title = "Giá tốt", Content = "Mua theo lốc tiện, chất lượng đều.", Overall = 4, Status = 1, CreateAt = new DateTime(2026, 7, 26, 19, 0, 0), Delete = false },
            new Review { Id = 12, OrderItemId = 16, ProductVariantId = 9, Title = "Ổn", Content = "Đúng chuẩn outdoor, màu rõ.", Overall = 4, Status = 1, CreateAt = new DateTime(2026, 7, 28, 7, 0, 0), Delete = false },
            // Product 4 — 4 reviews (items 5,10,11,17)
            new Review { Id = 13, OrderItemId = 5, ProductVariantId = 10, Title = "Thoải mái", Content = "Size S vừa, vải mát.", Overall = 5, Status = 1, CreateAt = new DateTime(2026, 7, 12, 20, 0, 0), Delete = false },
            new Review { Id = 14, OrderItemId = 10, ProductVariantId = 11, Title = "Đẹp", Content = "Màu hồng nữ tính, co giãn tốt.", Overall = 5, Status = 1, CreateAt = new DateTime(2026, 7, 19, 21, 0, 0), Delete = false },
            new Review { Id = 15, OrderItemId = 11, ProductVariantId = 12, Title = "Ổn áp", Content = "Size L rộng vừa phải.", Overall = 4, Status = 1, CreateAt = new DateTime(2026, 7, 20, 8, 0, 0), Delete = false },
            new Review { Id = 16, OrderItemId = 17, ProductVariantId = 10, Title = "Mặc tập ổn", Content = "Không bí, di chuyển dễ.", Overall = 4, Status = 1, CreateAt = new DateTime(2026, 7, 26, 15, 0, 0), Delete = false },
            // Product 5 — 3 reviews (items 12,18,19)
            new Review { Id = 17, OrderItemId = 12, ProductVariantId = 13, Title = "Chất Selkirk", Content = "Form đẹp, may chắc chắn.", Overall = 5, Status = 1, CreateAt = new DateTime(2026, 7, 19, 22, 0, 0), Delete = false },
            new Review { Id = 18, OrderItemId = 18, ProductVariantId = 14, Title = "Hồng xinh", Content = "Màu đẹp, mặc thoải mái.", Overall = 5, Status = 1, CreateAt = new DateTime(2026, 7, 26, 16, 0, 0), Delete = false },
            new Review { Id = 19, OrderItemId = 19, ProductVariantId = 13, Title = "Đáng mua", Content = "Quần bền, hợp chơi lâu.", Overall = 4, Status = 1, CreateAt = new DateTime(2026, 7, 27, 10, 0, 0), Delete = false }
        );
    }
}
