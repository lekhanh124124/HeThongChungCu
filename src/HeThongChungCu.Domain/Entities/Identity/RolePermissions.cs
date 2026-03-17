using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public static class RolePermissions
{
    public static readonly Dictionary<Role, HashSet<Permission>> Permissions = new()
    {
        [Role.Admin] = new HashSet<Permission>
        {
            Permission.UsersRead,
            Permission.UsersWrite,
            Permission.UsersDelete,
            Permission.UsersManageRoles
        },
        [Role.Manager] = new HashSet<Permission>
        {
            Permission.UsersRead,
            Permission.UsersWrite
        },
        [Role.Resident] = new HashSet<Permission>
        {
            Permission.UsersRead
        },
        [Role.Staff] = new HashSet<Permission>
        {
            Permission.UsersRead
        }
    };
}
