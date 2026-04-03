using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Interfaces;

public interface IIdentityDomainService
{
    /// <summary>
    /// Liên kết tài khoản với cư dân và thăng cấp vai trò (Guest -> Resident).
    /// </summary>
    Result LinkAccountToResident(TaiKhoan account, int residentId);

    /// <summary>
    /// Thu hồi các token định danh (UserCode) đang hoạt động.
    /// </summary>
    void RevokeIdentificationTokens(TaiKhoan account, ReasonRevoked reason);

    /// <summary>
    /// Xác nhận định danh bằng token, liên kết tài khoản và thu hồi token đã sử dụng.
    /// </summary>
    Result VerifyAndLinkAccount(
        TaiKhoan account, 
        string tokenHash, 
        int residentId, 
        DateTimeOffset revokedAt);
}
