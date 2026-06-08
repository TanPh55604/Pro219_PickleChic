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
    }
}