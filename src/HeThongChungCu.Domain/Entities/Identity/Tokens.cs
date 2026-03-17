using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class Tokens : BaseEntity
{
    public string RefreshToken { get; private set; } = null!; // Note: this field is used for both refresh tokens and reset password codes. 
    public DateTimeOffset ExpiresDate { get; private set; }
    public int UserId { get; private set; }
    public TokenType TokenType { get; private set; } = TokenType.RefreshToken;

    // Security fields
    public bool IsRevoked { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public string? NewAccessToken { get; private set; }
    public ReasonRevoked? ReasonRevoked { get; private set; }

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresDate;
    public bool IsActive => !IsRevoked && !IsExpired;

    public User User { get; private set; } = null!;

    private Tokens() { } // EF core

    public Tokens(int userId, string tokenValue, DateTimeOffset expiresDate, TokenType tokenType)
    {
        UserId = userId;
        RefreshToken = tokenValue; // Reuse the field
        ExpiresDate = expiresDate;
        TokenType = tokenType;
    }

    public static Tokens CreateResetPasswordToken(int userId, string code, DateTimeOffset expiresDate)
    {
        return new Tokens(userId, code, expiresDate, TokenType.ResetPasswordCode);
    }

    public static Tokens CreateRefreshToken(int userId, string token, DateTimeOffset expiresDate)
    {
        return new Tokens(userId, token, expiresDate, TokenType.RefreshToken);
    }

    public void Revoke(DateTimeOffset revokedAt, ReasonRevoked reason, string? newAccessToken = null)
    {
        IsRevoked = true;
        RevokedAt = revokedAt;
        ReasonRevoked = reason;
        NewAccessToken = newAccessToken;
    }
}
