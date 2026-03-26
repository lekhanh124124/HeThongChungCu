using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class Tokens : BaseEntity
{
    public string TokenHash { get; private set; } = string.Empty;
    public DateTimeOffset ExpiresDate { get; private set; }
    public int AccountId { get; private set; }
    public TokenType TokenType { get; private set; } = TokenType.RefreshToken;

    // Security fields
    public bool IsRevoked { get; internal set; }
    public DateTimeOffset? RevokedAt { get; internal set; }
    public ReasonRevoked? ReasonRevoked { get; internal set; }

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresDate;
    public bool IsActive => !IsRevoked && !IsExpired;

    public int TaiKhoanId { get; private set; }
    public TaiKhoan TaiKhoan { get; private set; } = null!;

    private Tokens() { } // EF core

    internal Tokens(int taiKhoanId, string tokenHash, DateTimeOffset expiresDate, TokenType tokenType)
    {
        TaiKhoanId = taiKhoanId;
        TokenHash = tokenHash;
        ExpiresDate = expiresDate;
        TokenType = tokenType;
    }
}
