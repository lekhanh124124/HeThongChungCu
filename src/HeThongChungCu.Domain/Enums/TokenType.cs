using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public sealed class TokenType : BaseEnum<TokenType, int>
{
    public static readonly TokenType RefreshToken = new(1, nameof(RefreshToken));
    public static readonly TokenType ResetPasswordCode = new(2, nameof(ResetPasswordCode));
    public static readonly TokenType UserCode = new(3, nameof(UserCode));
    
    private TokenType(int value, string name) : base(value, name)
    {
    }
}
