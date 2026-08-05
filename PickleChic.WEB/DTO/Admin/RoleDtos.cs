namespace PickleChic.WEB.DTO.Admin
{
    public class RoleResponse
    {
        public int Id { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public string? Permissions { get; set; }

        public int Status { get; set; }

        public bool IsEdit { get; set; } = true;
    }

    public class RoleCreateRequest
    {
        public string RoleName { get; set; } = string.Empty;

        public string? Permissions { get; set; }

        public int Status { get; set; } = 1;
    }

    public class RoleUpdateRequest : RoleCreateRequest
    {
        public int Id { get; set; }
    }

    public class PagePermissionItem
    {
        public string PageCode { get; set; } = string.Empty;

        public string PagePermissions { get; set; } = string.Empty;
    }
}
