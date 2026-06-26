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

            public const string GetAllWithDetails = "management/product/get-all-with-details";
            public const string Create = "management/product/create";
            public const string Update = "management/product/update";

            public static string GetByIdWithDetails(int id) => $"management/product/get-by-id-with-details/{id}";

            public static string Delete(int id) => $"management/product/delete/{id}";
        }

        public static class ProductVariant
        {
            public const string CreateWithAttributes = "management/product-variant/create-with-attributes";
            public const string UpdateWithAttributes = "management/product-variant/update-with-attributes";

            public static string GetByIdWithDetails(int id) => $"management/product-variant/get-by-id-with-details/{id}";

            public static string Delete(int id) => $"management/product-variant/delete/{id}";
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
    }
}