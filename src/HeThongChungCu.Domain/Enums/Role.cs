using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class Role : BaseEnum<Role, int>
{
    public static readonly Role Admin = new(1, "Quản trị viên");
    public static readonly Role Manager = new(2, "Quản lý");
    public static readonly Role Resident = new(3, "Cư dân");
    public static readonly Role Staff = new(4, "Nhân viên");
    public static readonly Role Guest = new(5, "Khách");

    private Role(int value, string name) : base(value, name)
    {
    }

}
