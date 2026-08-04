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

        public static class Role
        {
            public const string GetAll = "management/role/get-all";
            public const string Create = "management/role/create";
            public const string Update = "management/role/update";

            public static string GetById(int id) => $"management/role/get-by-id/{id}";

            public static string Delete(int id) => $"management/role/delete/{id}";
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

            public const string PercentReward = "management/rank/percent-reward";
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

        public static class PublicAttribute
        {
            public const string GetAll = "product-attribute/get-all";

            public static string GetAllByCategoryId(int categoryId) =>
                $"product-attribute/get-all-by-categoryId?categoryId={categoryId}";
        }

        public static class Product
        {
            public const string Filter = "product/filter";

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

        public static class Wishlist
        {
            public const string Create = "wishlist/create";

            public const string ManagementGetAll = "management/wishlist/get-all";

            public static string GetAllByUserId(int userId) =>
                $"wishlist/get-all-by-userId/{userId}";

            public static string Delete(int id) => $"wishlist/delete/{id}";
        }

        public static class PointHistory
        {
            public static string GetByCustomer(int customerId) =>
                $"point-history/customer/{customerId}";
        }

        public static class Review
        {
            public const string Create = "review/create";

            public const string Unreviewed = "review/customer/unreviewed";

            public static string ByVariant(int productVariantId) =>
                $"review/variant/{productVariantId}";

            public static string Eligibility(int productVariantId) =>
                $"review/variant/{productVariantId}/eligibility";

            public static class Management
            {
                public const string UpdateStatus = "management/review/update-status";

                public static string GetAll(string? keyword = null, int? status = null)
                {
                    var queryParts = new List<string>();

                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        queryParts.Add($"keyword={Uri.EscapeDataString(keyword.Trim())}");
                    }

                    if (status.HasValue)
                    {
                        queryParts.Add($"status={status.Value}");
                    }

                    return queryParts.Count == 0
                        ? "management/review/get-all"
                        : $"management/review/get-all?{string.Join("&", queryParts)}";
                }

                public static string GetById(int id) => $"management/review/get-by-id/{id}";

                public static string Delete(int id) => $"management/review/delete/{id}";
            }
        }

        public static class Order
        {
            public const string GetAll = "management/order/get-all";

            public const string CalculateTotal = "order/CalculateTotal";

            public const string CalculateTotalPOS = "order/CalculateTotalPOS";

            public const string PosCheckout = "order/POS-Checkout";

            public static string PaymentSuccess(int orderId, bool pos = false) =>
                $"order/PaymentSuccess?orderId={orderId}&pos={pos.ToString().ToLowerInvariant()}";

            public static string PaymentCanceled(int orderId) =>
                $"order/PaymentCanceled?orderId={orderId}";

            public static string UserDetail(int orderId) => $"order/user/detail/{orderId}";

            public static string UserCancel(int orderId) => $"order/user/cancel/{orderId}";

            public const string UserList = "order/user/list";

            public static string Lookup(
                string? orderCode = null,
                string? name = null,
                string? phoneNumber = null)
            {
                var parameters = new List<string>();

                if (!string.IsNullOrWhiteSpace(orderCode))
                {
                    parameters.Add($"orderCode={Uri.EscapeDataString(orderCode.Trim())}");
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    parameters.Add($"name={Uri.EscapeDataString(name.Trim())}");
                }

                if (!string.IsNullOrWhiteSpace(phoneNumber))
                {
                    parameters.Add($"phoneNumber={Uri.EscapeDataString(phoneNumber.Trim())}");
                }

                return parameters.Count == 0
                    ? "order/lookup"
                    : $"order/lookup?{string.Join("&", parameters)}";
            }

            public static string GetById(int id) => $"management/order/get-by-id/{id}";

            public static string UpdateStatus(int id) => $"management/order/update-status/{id}";

            public static string Checkout(
                decimal discountAmount,
                decimal shippingFee,
                int paymentMethodTypeId,
                int addressId,
                int? voucherId = null,
                string? note = null,
                bool usePoints = false,
                bool bopis = false)
            {
                var url =
                    $"order/Checkout?discountAmount={discountAmount}&shippingFee={shippingFee}&PaymentMethodTypeId={paymentMethodTypeId}&addressId={addressId}&usePoints={usePoints}&bopis={bopis}";

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

        public static class Pos
        {
            public static string Products(
                string? keyword = null,
                int? brandId = null,
                int? categoryId = null,
                int pageNumber = 1,
                int pageSize = 20)
            {
                var query = new List<string>
                {
                    $"pageNumber={pageNumber}",
                    $"pageSize={pageSize}"
                };

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    query.Add($"keyword={Uri.EscapeDataString(keyword.Trim())}");
                }

                if (brandId is > 0)
                {
                    query.Add($"brandId={brandId.Value}");
                }

                if (categoryId is > 0)
                {
                    query.Add($"categoryId={categoryId.Value}");
                }

                return $"management/pos/products?{string.Join("&", query)}";
            }

            public static string CheckStock(int variantId, int quantity) =>
                $"management/pos/products/{variantId}/stock?quantity={quantity}";

            public static string Customers(
                string? keyword = null,
                int pageNumber = 1,
                int pageSize = 20)
            {
                var query = new List<string>
                {
                    $"pageNumber={pageNumber}",
                    $"pageSize={pageSize}"
                };

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    query.Add($"keyword={Uri.EscapeDataString(keyword.Trim())}");
                }

                return $"management/pos/customers?{string.Join("&", query)}";
            }

            public static string Vouchers(int customerId) =>
                $"management/pos/vouchers?customerId={customerId}";
        }

        public static class Report
        {
            public static string Statistics(DateTime? startDate = null, DateTime? endDate = null, string? groupBy = null)
            {
                var url = "management/report/statistics";

                var queryParts = new List<string>();

                if (startDate.HasValue)
                {
                    queryParts.Add(
                        $"startDate={startDate.Value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)}");
                }

                if (endDate.HasValue)
                {
                    queryParts.Add(
                        $"endDate={endDate.Value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)}");
                }

                if (!string.IsNullOrWhiteSpace(groupBy))
                {
                    queryParts.Add($"groupBy={Uri.EscapeDataString(groupBy.Trim())}");
                }

                if (queryParts.Count == 0)
                {
                    return url;
                }

                return $"{url}?{string.Join("&", queryParts)}";
            }
        }
    }
}