namespace PickleChic.WEB.Constant
{
    public static class EndPointConfig
    {
        public static class Auth
        {
            public const string LoginCustomer = "Access/LoginCustomer";

            public const string LoginStaff = "Access/LoginStaff";

            public const string CustomerRegister = "Access/customer-register";

            public const string ResetPassword = "Access/reset-password";

            public const string ChangePasswordCustomer = "Access/change-password";

            public const string ChangePasswordStaff = "Access/staff-change-password";

            public const string Check = "Access/Check";
        }

        public static class Staff
        {
            public const string GetAll = "management/staff/get-all";
            public const string Create = "management/staff/create";
            public const string Update = "management/staff/update";

            public static string GetById(int id) => $"management/staff/get-by-id/{id}";

            public static string Delete(int id) => $"management/staff/delete/{id}";
        }

        public static class PublicCategory
        {
            public const string GetAll = "category/get-all";

            public static string GetById(int id) => $"category/get-by-id/{id}";
        }

        public static class PublicBrand
        {
            public const string GetAll = "brand/get-all";

            public static string GetById(int id) => $"brand/get-by-id/{id}";
        }

        public static class Category
        {
            public const string GetAll = "management/category/get-all";
            public const string Create = "management/category/create";
            public const string Update = "management/category/update";

            public static string GetById(int id) => $"management/category/get-by-id/{id}";

            public static string Delete(int id) => $"management/category/delete/{id}";

            public static string UploadImage(int id) => $"management/category/upload-image/{id}";

            public static string DeleteImage(int id) => $"management/category/delete-image/{id}";
        }

        public static class Brand
        {
            public const string GetAll = "management/brand/get-all";
            public const string Create = "management/brand/create";
            public const string Update = "management/brand/update";

            public static string GetById(int id) => $"management/brand/get-by-id/{id}";

            public static string Delete(int id) => $"management/brand/delete/{id}";
        }

        public static class Rank
        {
            public const string GetAll = "management/rank/get-all";
            public const string Create = "management/rank/create";
            public const string Update = "management/rank/update";

            public static string GetById(int id) => $"management/rank/get-by-id/{id}";

            public static string Delete(int id) => $"management/rank/delete/{id}";

            public static string UpdatePercentReward(double value) =>
                $"management/rank/percent-reward?value={value.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
        }

        public static class Attribute
        {
            public const string GetAll = "management/product-attribute/get-all";
            public const string CreateWithValues = "management/product-attribute/create-with-values";
            public const string Update = "management/product-attribute/update";
            public const string ModifyWithFlag = "management/product-attribute/modify-with-flag";

            public static string GetById(int id) => $"management/product-attribute/get-by-id/{id}";

            public static string GetAllByCategoryId(int categoryId) =>
                $"management/product-attribute/get-all-by-categoryId?categoryId={categoryId}";

            public static string Delete(int id) => $"management/product-attribute/delete/{id}";
        }

        public static class AttributeValue
        {
            public const string Create = "management/attribute-value/create";
            public const string Update = "management/attribute-value/update";

            public static string Delete(int id) => $"management/attribute-value/delete/{id}";
        }

        public static class Product
        {
            public const string Search = "product-variant/search";

            public static string GetByIdWithDetailsPublic(int id) => $"product/get-by-id-with-details/{id}";

            public const string GetAllWithDetails = "management/product/get-all-with-details";
            public const string Create = "management/product/create";
            public const string Update = "management/product/update";

            public static string GetByIdWithDetails(int id) => $"management/product/get-by-id-with-details/{id}";

            public static string Delete(int id) => $"management/product/delete/{id}";
        }

        public static class ProductVariant
        {
            public static string GetByCategory(int categoryId, string? sortBy = null)
            {
                var url = $"product-variant/get-by-category/{categoryId}";

                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    url += $"?sortBy={Uri.EscapeDataString(sortBy)}";
                }

                return url;
            }

            public static string GetByBrand(int brandId, string? sortBy = null)
            {
                var url = $"product-variant/get-by-brand/{brandId}";

                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    url += $"?sortBy={Uri.EscapeDataString(sortBy)}";
                }

                return url;
            }

            public const string CreateWithAttributes = "management/product-variant/create-with-attributes";
            public const string UpdateWithAttributes = "management/product-variant/update-with-attributes";

            public static string GetByIdWithDetails(int id) => $"management/product-variant/get-by-id-with-details/{id}";

            public static string Delete(int id) => $"management/product-variant/delete/{id}";
        }

        public static class ProductVariantImage
        {
            public const string Upload = "management/product-variant-image/upload";

            public static string GetByVariant(int variantId) =>
                $"management/product-variant-image/get-by-variant/{variantId}";

            public static string SetMain(int id) => $"management/product-variant-image/set-main/{id}";

            public static string Delete(int id) => $"management/product-variant-image/delete/{id}";
        }

        public static class Cart
        {
            public const string Create = "cart-item/create";

            public const string Update = "cart-item/update";

            public static string GetByUser(int customerId) => $"cart-item/get-by-user/{customerId}";

            public static string Delete(int id) => $"cart-item/delete/{id}";
        }

        public static class Address
        {
            public const string Create = "address/create";

            public const string Update = "address/update";

            public const string Provinces = "address/provinces";

            public static string GetById(int id) => $"address/get-by-id/{id}";

            public static string GetByUser(int customerId) => $"address/get-by-user/{customerId}";

            public static string Delete(int id) => $"address/delete/{id}";

            public static string DistrictsByProvince(int provinceId) =>
                $"address/districts-by-province/{provinceId}";

            public static string WardsByDistrict(int districtId) =>
                $"address/wards-by-district/{districtId}";
        }

        public static class Voucher
        {
            public const string GetAvailable = "voucher/get-available-voucher";

            public static string GetById(int id) => $"voucher/get-by-id/{id}";

            public static class Management
            {
                public const string GetAll = "management/voucher/get-all";
                public const string Create = "management/voucher/create";
                public const string Update = "management/voucher/update";

                public static string GetById(int id) => $"management/voucher/get-by-id/{id}";

                public static string Delete(int id) => $"management/voucher/delete/{id}";
            }
        }

        public static class PointHistory
        {
            public static string GetByCustomer(int customerId) =>
                $"point-history/customer/{customerId}";
        }

        public static class Order
        {
            public const string GetAll = "management/order/get-all";

            public const string CalculateTotal = "order/CalculateTotal";

            public static string PaymentSuccess(int orderId, bool pos = false) =>
                $"order/PaymentSuccess?orderId={orderId}&pos={pos.ToString().ToLowerInvariant()}";

            public static string PaymentCanceled(int orderId) =>
                $"order/PaymentCanceled?orderId={orderId}";

            public static string UserDetail(int orderId) => $"order/user/detail/{orderId}";

            public const string UserList = "order/user/list";

            public static string GetById(int id) => $"management/order/get-by-id/{id}";

            public static string UpdateStatus(int id) => $"management/order/update-status/{id}";

            public static string Checkout(
                decimal discountAmount,
                decimal shippingFee,
                int paymentMethodTypeId,
                int addressId,
                int? voucherId = null,
                string? note = null)
            {
                var url =
                    $"order/Checkout?discountAmount={discountAmount}&shippingFee={shippingFee}&PaymentMethodTypeId={paymentMethodTypeId}&addressId={addressId}";

                if (voucherId.HasValue)
                {
                    url += $"&voucherId={voucherId.Value}";
                }

                if (!string.IsNullOrWhiteSpace(note))
                {
                    url += $"&note={Uri.EscapeDataString(note)}";
                }

                return url;
            }
        }
    }
}