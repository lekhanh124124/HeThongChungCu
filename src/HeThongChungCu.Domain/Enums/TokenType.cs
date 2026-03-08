using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public sealed class TokenType : BaseEnum<TokenType, int>
{
    public static readonly TokenType RefreshToken = new(1, nameof(RefreshToken));
    public static readonly TokenType ResetPasswordCode = new(2, nameof(ResetPasswordCode));
    
    private TokenType(int value, string name) : base(value, name)
    {
    }
}
