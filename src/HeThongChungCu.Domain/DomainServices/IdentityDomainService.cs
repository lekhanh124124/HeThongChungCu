using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Interfaces;

namespace HeThongChungCu.Domain.DomainServices;

public class IdentityDomainService : IIdentityDomainService
{
    public Result CanLinkAccountToResident(TaiKhoan account, int residentId, bool isResidentAlreadyLinked)
    {
        // 1. Kiểm tra cư dân đã có tài khoản hay chưa
        if (isResidentAlreadyLinked && account.NguoiDungId != residentId)
        {
            return Result.Failure(AuthErrors.ResidentAlreadyLinked);
        }

        // 2. Kiểm tra tài khoản đã liên kết hay chưa
        if (account.NguoiDungId != null && account.NguoiDungId != residentId)
        {
            return Result.Failure(AuthErrors.AccountAlreadyLinked);
        }

        return Result.Success();
    }

    public void LinkAccountToResident(TaiKhoan account, int residentId)
    {
        if (account.NguoiDungId == null)
        {
            account.LinkToUser(residentId);
        }

        // Thăng cấp vai trò nếu là Guest
        var roles = account.PhanQuyens.Select(pq => pq.RoleId).ToList();
        if (roles.Contains(Role.Guest) && !roles.Contains(Role.Resident))
        {
            account.RemoveRole(Role.Guest);
            account.AddRole(Role.Resident);
        }
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
}
