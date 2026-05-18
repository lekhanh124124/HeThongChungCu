using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class Role : BaseEnum<Role, int>
{
    public static readonly Role Admin = new(1, "Admin");
    public static readonly Role Manager = new(2, "Manager");
    public static readonly Role Resident = new(3, "Resident");
    public static readonly Role Staff = new(4, "Staff");
    public static readonly Role Guest = new(5, "Guest");

    private Role(int value, string name) : base(value, name)
    {
    }

}
