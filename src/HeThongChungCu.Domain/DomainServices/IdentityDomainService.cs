using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Interfaces;

namespace HeThongChungCu.Domain.DomainServices;

public class IdentityDomainService : IIdentityDomainService
{
    public Result LinkAccountToResident(TaiKhoan account, int residentId)
    {
        // 1. Kiểm tra tài khoản đã liên kết hay chưa
        if (account.NguoiDungId != null && account.NguoiDungId != residentId)
        {
            return Result.Failure(AuthErrors.AccountAlreadyLinked);
        }

        if (account.NguoiDungId == null)
        {
            account.LinkToUser(residentId);
        }

        // 2. Thăng cấp vai trò nếu là Guest
        var roles = account.PhanQuyens.Select(pq => pq.RoleId).ToList();
        if (roles.Contains(Role.Guest) && !roles.Contains(Role.Resident))
        {
            account.RemoveRole(Role.Guest);
            account.AddRole(Role.Resident);
        }

        return Result.Success();
    }

    public void RevokeIdentificationTokens(TaiKhoan account, ReasonRevoked reason)
    {
        var pendingTokens = account.Tokens
            .Where(t => t.TokenType == TokenType.UserCode && t.IsActive)
            .ToList();

        foreach (var token in pendingTokens)
        {
            account.RevokeToken(token.TokenHash, DateTimeOffset.UtcNow, reason);
        }
    }

    public Result VerifyAndLinkAccount(TaiKhoan account, string tokenHash, int residentId, DateTimeOffset revokedAt)
    {
        // 1. Kiểm tra Token
        var tokenEntity = account.Tokens.FirstOrDefault(t => t.TokenHash == tokenHash && t.TokenType == TokenType.UserCode);
        if (tokenEntity == null || !tokenEntity.IsActive)
        {
            return Result.Failure(AuthErrors.InvalidToken);
        }

        // 2. Liên kết tài khoản và thăng cấp vai trò
        var linkResult = LinkAccountToResident(account, residentId);
        if (linkResult.IsFailure)
        {
            return linkResult;
        }

        // 3. Thu hồi token hiện tại
        account.RevokeToken(tokenHash, revokedAt, ReasonRevoked.UserAction);

        return Result.Success();
    }
}
