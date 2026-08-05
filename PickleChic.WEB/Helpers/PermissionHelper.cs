using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Model;

namespace PickleChic.WEB.Helpers;

public static class PermissionHelper
{
    public const string Create = "C";
    public const string Read = "R";
    public const string Edit = "U";
    public const string Delete = "D";
    public const string Approve = "A";

    private static readonly Dictionary<string, string> PageCodeMapping = new(StringComparer.OrdinalIgnoreCase)
    {
        { "/admin/dashboard", "DASHBOARD" },
        { "/admin/statistical", "DASHBOARD" },
        { "/admin/staffs", "STAFF" },
        { "/admin/permissions", "PERMISSIONS" },
        { "/admin/products", "PRODUCT" },
        { "/admin/product-variants", "PRODUCT" },
        { "/admin/categories", "CATEGORY" },
        { "/admin/brands", "BRAND" },
        { "/admin/attributes", "ATTRIBUTE" },
        { "/admin/attribute-values", "ATTRIBUTE" },
        { "/admin/vouchers", "VOUCHER" },
        { "/admin/orders", "ORDER" },
        { "/admin/offline-sales", "OFFLINE" },
        { "/admin/reviews", "REVIEW" },
        { "/admin/ranks", "RANK" },
        { "/admin/customers", "CUSTOMER" }
    };

    public static IReadOnlyDictionary<string, string> GetModules() => PageCodeMapping;

    public static IEnumerable<(string PageCode, string Route)> GetUniqueModules()
    {
        return PageCodeMapping
            .GroupBy(x => x.Value)
            .Select(g => (g.Key, g.OrderBy(x => x.Key.Length).First().Key));
    }

    public static string GetPageCode(string? currentPath)
    {
        if (string.IsNullOrWhiteSpace(currentPath))
        {
            return string.Empty;
        }

        var path = currentPath.Split('?')[0].ToLowerInvariant();
        if (!path.StartsWith('/'))
        {
            path = "/" + path;
        }

        if (path.Length > 1)
        {
            path = path.TrimEnd('/');
        }

        foreach (var entry in PageCodeMapping.OrderByDescending(x => x.Key.Length))
        {
            var routeTarget = entry.Key.ToLowerInvariant();
            if (path == routeTarget || path.StartsWith(routeTarget + "/"))
            {
                return entry.Value;
            }
        }

        return string.Empty;
    }

    public static bool HasAccess(AuthModel? user, string pageCode, string action)
    {
        if (user?.PagePermissions is null || string.IsNullOrWhiteSpace(pageCode) || string.IsNullOrWhiteSpace(action))
        {
            return false;
        }

        var permission = user.PagePermissions
            .FirstOrDefault(x => string.Equals(x.PageCode, pageCode, StringComparison.OrdinalIgnoreCase));

        if (permission is null || string.IsNullOrWhiteSpace(permission.PagePermissions))
        {
            return false;
        }

        return permission.PagePermissions.Contains(action, StringComparison.OrdinalIgnoreCase);
    }

    public static string GetActionFromPath(string path)
    {
        var p = path.ToLowerInvariant();
        if (p.Contains("/create") || p.Contains("/add"))
        {
            return Create;
        }

        if (p.Contains("/edit") || p.Contains("/update"))
        {
            return Edit;
        }

        if (p.Contains("/delete"))
        {
            return Delete;
        }

        return Read;
    }

    public static bool SupportsAction(string pageCode, string action)
    {
        if (IsOrderModule(pageCode))
        {
            return action is Read or Approve;
        }

        if (IsReadOnlyModule(pageCode))
        {
            return action == Read;
        }

        return action is Create or Read or Edit or Delete;
    }

    public static bool IsOrderModule(string pageCode) =>
        string.Equals(pageCode, "ORDER", StringComparison.OrdinalIgnoreCase);

    public static bool IsReadOnlyModule(string pageCode) =>
        string.Equals(pageCode, "DASHBOARD", StringComparison.OrdinalIgnoreCase);

    public static string GetDisplayName(string pageCode) => pageCode.ToUpperInvariant() switch
    {
        "DASHBOARD" => "Thống kê",
        "STAFF" => "Quản trị viên",
        "PERMISSIONS" => "Phân quyền",
        "PRODUCT" => "Sản phẩm",
        "CATEGORY" => "Thể loại",
        "BRAND" => "Thương hiệu",
        "ATTRIBUTE" => "Thuộc tính",
        "VOUCHER" => "Mã giảm giá",
        "ORDER" => "Đơn hàng",
        "OFFLINE" => "Bán hàng offline",
        "REVIEW" => "Đánh giá",
        "RANK" => "Xếp hạng",
        "CUSTOMER" => "Khách hàng",
        _ => pageCode
    };

    public static string FormatActions(string? permissions)
    {
        if (string.IsNullOrWhiteSpace(permissions))
        {
            return string.Empty;
        }

        var parts = new List<string>();
        if (permissions.Contains(Read, StringComparison.OrdinalIgnoreCase)) parts.Add("Xem");
        if (permissions.Contains(Create, StringComparison.OrdinalIgnoreCase)) parts.Add("Thêm");
        if (permissions.Contains(Edit, StringComparison.OrdinalIgnoreCase)) parts.Add("Sửa");
        if (permissions.Contains(Delete, StringComparison.OrdinalIgnoreCase)) parts.Add("Xóa");
        if (permissions.Contains(Approve, StringComparison.OrdinalIgnoreCase)) parts.Add("Duyệt");
        return string.Join("|", parts);
    }

    public static string BuildAdminPermissionsJson()
    {
        var items = GetUniqueModules()
            .Select(m => new PagePermissionItem
            {
                PageCode = m.PageCode,
                PagePermissions = IsOrderModule(m.PageCode)
                    ? $"{Read}{Approve}"
                    : IsReadOnlyModule(m.PageCode)
                        ? Read
                        : $"{Create}{Read}{Edit}{Delete}"
            })
            .ToList();

        return System.Text.Json.JsonSerializer.Serialize(items);
    }
}
