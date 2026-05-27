namespace PickleChic.WEB.Constant
{
    public static class RouterConfig
    {
        // Error
        public const string AccessDenied = "/403";
        public const string NotFound = "/404";
        public const string ServerError = "/500";
        public const string BadRequest = "/400";

        public static class Customer
        {
            // Home
            public const string Home = "/";

            // Search
            public const string Search = "/search";

            // Auth
            public const string SignUp = "/register";
            public const string Login = "/login";
            public const string ForgotPassword = "/forgot-password";
            public const string ChangePassword = "/change-password";

            // Product
            public const string Products = "/products";
            public const string ProductDetail = "/products/detail/{id:int}";

            // Cart
            public const string Cart = "/cart";

            // Checkout
            public const string Checkout = "/checkout";

            // Order
            public const string Orders = "/orders";
            public const string OrderDetail = "/orders/detail/{id:int}";
            public const string PaymentSuccess = "/orders/payment-success";
            public const string PaymentCancelled = "/orders/payment-cancelled";
            public const string SearchOrder = "/orders/search";

            // Wishlist
            public const string Wishlists = "/wishlist";

            // Profile
            public const string Profile = "/profile";
            public const string Addresses = "/profile/addresses";
            public const string Points = "/profile/points";
            public const string Reviews = "/profile/reviews";
        }

        public static class Admin
        {
            // Root
            public const string Root = "/admin";

            // Home - không bắt quyền
            public const string Home = "/admin/home";

            // Auth
            public const string Login = "/admin/login";

            // Dashboard / Statistical
            public const string Dashboard = "/admin/dashboard";
            public const string Statistical = "/admin/statistical";

            // =========================
            // Product
            // =========================
            public const string Products = "/admin/products";
            public const string CreateProduct = "/admin/products/create";
            public const string ProductDetail = "/admin/products/detail/{id:int}";
            public const string EditProduct = "/admin/products/edit/{id:int}";

            // =========================
            // Product Variant
            // =========================
            public const string ProductVariants = "/admin/product-variants";
            public const string CreateProductVariant = "/admin/product-variants/create";
            public const string ProductVariantDetail = "/admin/product-variants/detail/{id:int}";
            public const string EditProductVariant = "/admin/product-variants/edit/{id:int}";

            // Product Variant Attributes
            public const string ProductVariantAttributes = "/admin/product-variant-attributes";
            public const string CreateProductVariantAttribute = "/admin/product-variant-attributes/create";
            public const string ProductVariantAttributeDetail = "/admin/product-variant-attributes/detail/{id:int}";
            public const string EditProductVariantAttribute = "/admin/product-variant-attributes/edit/{id:int}";

            // =========================
            // Category
            // =========================
            public const string Categories = "/admin/categories";
            public const string CreateCategory = "/admin/categories/create";
            public const string CategoryDetail = "/admin/categories/detail/{id:int}";
            public const string EditCategory = "/admin/categories/edit/{id:int}";

            // =========================
            // Brand
            // =========================
            public const string Brands = "/admin/brands";
            public const string CreateBrand = "/admin/brands/create";
            public const string BrandDetail = "/admin/brands/detail/{id:int}";
            public const string EditBrand = "/admin/brands/edit/{id:int}";

            // =========================
            // Attribute
            // =========================
            public const string Attributes = "/admin/attributes";
            public const string CreateAttribute = "/admin/attributes/create";
            public const string AttributeDetail = "/admin/attributes/detail/{id:int}";
            public const string EditAttribute = "/admin/attributes/edit/{id:int}";

            // Attribute Value
            public const string AttributeValues = "/admin/attribute-values";
            public const string CreateAttributeValue = "/admin/attribute-values/create";
            public const string AttributeValueDetail = "/admin/attribute-values/detail/{id:int}";
            public const string EditAttributeValue = "/admin/attribute-values/edit/{id:int}";

            // =========================
            // Customer
            // =========================
            public const string Customers = "/admin/customers";
            public const string CreateCustomer = "/admin/customers/create";
            public const string CustomerDetail = "/admin/customers/detail/{id:int}";
            public const string EditCustomer = "/admin/customers/edit/{id:int}";

            // Address
            public const string Addresses = "/admin/addresses";
            public const string AddressDetail = "/admin/addresses/detail/{id:int}";
            public const string EditAddress = "/admin/addresses/edit/{id:int}";

            // Wishlist
            public const string Wishlists = "/admin/wishlists";
            public const string WishlistDetail = "/admin/wishlists/detail/{id:int}";

            // Cart
            public const string CartItems = "/admin/cart-items";
            public const string CartItemDetail = "/admin/cart-items/detail/{id:int}";

            // Rank
            public const string Ranks = "/admin/ranks";
            public const string CreateRank = "/admin/ranks/create";
            public const string RankDetail = "/admin/ranks/detail/{id:int}";
            public const string EditRank = "/admin/ranks/edit/{id:int}";

            // Point History
            public const string PointHistories = "/admin/point-histories";
            public const string PointHistoryDetail = "/admin/point-histories/detail/{id:int}";

            // =========================
            // Staff / Admin Account
            // =========================
            public const string Staffs = "/admin/staffs";
            public const string CreateStaff = "/admin/staffs/create";
            public const string StaffDetail = "/admin/staffs/detail/{id:int}";
            public const string EditStaff = "/admin/staffs/edit/{id:int}";

            // Role
            public const string Roles = "/admin/roles";
            public const string CreateRole = "/admin/roles/create";
            public const string RoleDetail = "/admin/roles/detail/{id:int}";
            public const string EditRole = "/admin/roles/edit/{id:int}";

            // Page Permission
            public const string PagePermissions = "/admin/page-permissions";
            public const string CreatePagePermission = "/admin/page-permissions/create";
            public const string PagePermissionDetail = "/admin/page-permissions/detail/{id:int}";
            public const string EditPagePermission = "/admin/page-permissions/edit/{id:int}";

            // =========================
            // Order
            // =========================
            public const string Orders = "/admin/orders";
            public const string OrderDetail = "/admin/orders/detail/{id:int}";
            public const string EditOrder = "/admin/orders/edit/{id:int}";

            // Order Item
            public const string OrderItems = "/admin/order-items";
            public const string OrderItemDetail = "/admin/order-items/detail/{id:int}";

            // Offline Sale
            public const string OfflineSales = "/admin/offline-sales";
            public const string CreateOfflineSale = "/admin/offline-sales/create";
            public const string OfflineSaleDetail = "/admin/offline-sales/detail/{id:int}";

            // =========================
            // Payment
            // =========================
            public const string PaymentMethods = "/admin/payment-methods";
            public const string CreatePaymentMethod = "/admin/payment-methods/create";
            public const string PaymentMethodDetail = "/admin/payment-methods/detail/{id:int}";
            public const string EditPaymentMethod = "/admin/payment-methods/edit/{id:int}";

            public const string PaymentSuccess = "/admin/orders/payment-success";
            public const string PaymentCancelled = "/admin/orders/payment-cancelled";

            // =========================
            // Voucher
            // =========================
            public const string Vouchers = "/admin/vouchers";
            public const string CreateVoucher = "/admin/vouchers/create";
            public const string VoucherDetail = "/admin/vouchers/detail/{id:int}";
            public const string EditVoucher = "/admin/vouchers/edit/{id:int}";

            // =========================
            // Promotion
            // =========================
            public const string Promotions = "/admin/promotions";
            public const string CreatePromotion = "/admin/promotions/create";
            public const string PromotionDetail = "/admin/promotions/detail/{id:int}";
            public const string EditPromotion = "/admin/promotions/edit/{id:int}";

            // Promotion Detail
            public const string PromotionDetails = "/admin/promotion-details";
            public const string CreatePromotionDetail = "/admin/promotion-details/create";
            public const string PromotionDetailDetail = "/admin/promotion-details/detail/{id:int}";
            public const string EditPromotionDetail = "/admin/promotion-details/edit/{id:int}";

            // =========================
            // Review / Q&A / Search
            // =========================
            public const string Reviews = "/admin/reviews";
            public const string ReviewDetail = "/admin/reviews/detail/{id:int}";

            public const string Qna = "/admin/qna";
            public const string ChatbotQna = "/admin/chatbot-qna";
            public const string SearchManagement = "/admin/search-management";

            // =========================
            // Setting
            // =========================
            public const string Settings = "/admin/settings";
        }

        public static string BuildRoute(string route, int id)
        {
            return route.Replace("{id:int}", id.ToString());
        }
    }
}