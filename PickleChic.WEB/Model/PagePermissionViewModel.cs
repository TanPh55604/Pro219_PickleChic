namespace PickleChic.WEB.Model
{
    public class PagePermissionViewModel
    {
        public string PageCode { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public bool CanCreate { get; set; }

        public bool CanRead { get; set; }

        public bool CanUpdate { get; set; }

        public bool CanDelete { get; set; }

        public bool CanApprove { get; set; }

        public string GetPermissionString()
        {
            var value = string.Empty;
            if (CanCreate) value += "C";
            if (CanRead) value += "R";
            if (CanUpdate) value += "U";
            if (CanDelete) value += "D";
            if (CanApprove) value += "A";
            return value;
        }

        public bool HasAny => CanCreate || CanRead || CanUpdate || CanDelete || CanApprove;
    }
}
