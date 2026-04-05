using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Interfaces;

public interface IIdentityDomainService
{
    /// <summary>
    /// Kiểm tra xem tài khoản có thể liên kết với cư dân hay không.
    /// </summary>
    Result CanLinkAccountToResident(TaiKhoan account, int residentId, bool isResidentAlreadyLinked);

    /// <summary>
    /// Thực hiện liên kết tài khoản với cư dân và thăng cấp vai trò (Không kiểm tra lại điều kiện).
    /// </summary>
    void LinkAccountToResident(TaiKhoan account, int residentId);

    /// <summary>
    /// Thu hồi các token định danh (UserCode) đang hoạt động.
    /// </summary>
    void RevokeIdentificationTokens(TaiKhoan account, ReasonRevoked reason);
}
