using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class Permission : BaseEnum<Permission, string>
{
    public static readonly Permission UsersRead = new("users:read", nameof(UsersRead));
    public static readonly Permission UsersWrite = new("users:write", nameof(UsersWrite));
    public static readonly Permission UsersDelete = new("users:delete", nameof(UsersDelete));
    public static readonly Permission UsersManageRoles = new("users:manage_roles", nameof(UsersManageRoles));

    private Permission(string value, string name) : base(value, name)
    {
    }
}
