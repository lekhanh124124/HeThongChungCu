namespace HeThongChungCu.Domain.Enums;

using Ardalis.SmartEnum;

public sealed class TokenType : SmartEnum<TokenType>
{
    public static readonly TokenType RefreshToken = new(nameof(RefreshToken), 1);
    public static readonly TokenType ResetPasswordCode = new(nameof(ResetPasswordCode), 2);

    private TokenType(string name, int value) : base(name, value)
    {
    }
}
