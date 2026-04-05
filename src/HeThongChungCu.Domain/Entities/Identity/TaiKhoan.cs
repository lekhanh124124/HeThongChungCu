using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class TaiKhoan : AggregateRoot
{
    public string TenDangNhap { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string MatKhauHash { get; private set; } = string.Empty;
    public int? AnhDaiDienId { get; private set; }
    public virtual TepTaiLieu? AnhDaiDien { get; private set; }
    public bool IsActive { get; private set; }

    public int? NguoiDungId { get; private set; }

    private readonly List<PhanQuyen> _phanQuyens = [];
    public IReadOnlyCollection<PhanQuyen> PhanQuyens => _phanQuyens.AsReadOnly();

    private readonly List<Tokens> _tokens = [];
    public IReadOnlyCollection<Tokens> Tokens => _tokens.AsReadOnly();

    private TaiKhoan() { } // EF Core

    public TaiKhoan(int? nguoiDungId, string tenDangNhap, string email, string matKhauHash)
    {
        NguoiDungId = nguoiDungId;
        TenDangNhap = tenDangNhap;
        Email = new Email(email);
        MatKhauHash = matKhauHash;
        IsActive = true;
    }

    public void UpdateEmail(string email) => Email = new Email(email);


    public void LinkToUser(int nguoiDungId)
    {
        if (NguoiDungId != null)
        {
            throw new BusinessException("Tài khoản đã được liên kết với một người dùng.");
        }
        NguoiDungId = nguoiDungId;
    }

    public void UpdatePassword(string newPasswordHash) => MatKhauHash = newPasswordHash;

    public void UpdateAvatar(TepTaiLieu? avatar)
    {
        AnhDaiDien = avatar;
        AnhDaiDienId = avatar?.Id;
    }

    public void Deactivate() => IsActive = false;

    public void AddRole(Role role)
    {
        if (!_phanQuyens.Any(r => r.RoleId == role))
        {
            _phanQuyens.Add(new PhanQuyen(Id, role));
        }
    }

    public void RemoveRole(Role role)
    {
        var phanQuyen = _phanQuyens.FirstOrDefault(pq => pq.RoleId == role);
        if (phanQuyen != null)
        {
            _phanQuyens.Remove(phanQuyen);
        }
    }

    public void AddRefreshToken(string tokenHash, DateTimeOffset expiresDate)
    {
        var refreshToken = new Tokens(Id, tokenHash, expiresDate, TokenType.RefreshToken);
        _tokens.Add(refreshToken);
    }

    public void AddResetPasswordToken(string codeHash, DateTimeOffset expiresDate)
    {
        var resetToken = new Tokens(Id, codeHash, expiresDate, TokenType.ResetPasswordCode);
        _tokens.Add(resetToken);
    }

    public void AddUserCodeToken(string codeHash, DateTimeOffset expiresDate)
    {
        var userCodeToken = new Tokens(Id, codeHash, expiresDate, TokenType.UserCode);
        _tokens.Add(userCodeToken);
    }

    public void RemoveToken(string tokenHash)
    {
        var token = _tokens.FirstOrDefault(t => t.TokenHash == tokenHash);
        if (token != null)
        {
            _tokens.Remove(token);
        }
    }

    public void RevokeToken(string tokenHash, DateTimeOffset revokedAt, ReasonRevoked reason)
    {
        var token = _tokens.FirstOrDefault(t => t.TokenHash == tokenHash);
        if (token != null)
        {
            token.IsRevoked = true;
            token.RevokedAt = revokedAt;
            token.ReasonRevoked = reason;
        }
    }
}
