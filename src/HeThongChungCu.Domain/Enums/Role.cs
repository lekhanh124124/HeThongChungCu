using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class Role : BaseEnum<Role, int>
{
    public static readonly Role Admin = new(1, nameof(Admin));
    public static readonly Role Manager = new(2, nameof(Manager));
    public static readonly Role Resident = new(3, nameof(Resident));
    public static readonly Role Staff = new(4, nameof(Staff));

    private Role(int value, string name) : base(value, name)
    {
    }
}
