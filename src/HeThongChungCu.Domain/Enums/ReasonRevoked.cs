using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class ReasonRevoked : BaseEnum<ReasonRevoked, int>
{
    public static readonly ReasonRevoked Logout = new(1, nameof(Logout));
    public static readonly ReasonRevoked ReplacedByNewToken = new(2, nameof(ReplacedByNewToken));
    public static readonly ReasonRevoked Compromised = new(3, nameof(Compromised));
    public static readonly ReasonRevoked Expired = new(4, nameof(Expired));
    public static readonly ReasonRevoked UserAction = new(5, nameof(UserAction));
    public static readonly ReasonRevoked AdminAction = new(6, nameof(AdminAction));

    private ReasonRevoked(int value, string name) : base(value, name)
    {
    }
}
