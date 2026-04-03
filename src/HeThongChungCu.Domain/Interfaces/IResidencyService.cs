using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Interfaces;

public interface IResidencyService
{
    /// <summary>
    /// Kiểm tra tính duy nhất của CCCD và Số điện thoại.
    /// </summary>
    Result CheckUniqueness(bool cccdExists, bool phoneExists);

    /// <summary>
    /// Kiểm tra xem người dùng có quyền (Chủ hộ) để thực hiện yêu cầu cho căn hộ này không.
    /// </summary>
    Result CheckChuHoPermission(QuanHeCuTru? requesterRelation);

    /// <summary>
    /// Tạo người dùng mới từ yêu cầu cư trú.
    /// </summary>
    NguoiDung CreateUserFromRequest(YeuCauCuTru request);

    /// <summary>
    /// Cập nhật thông tin người dùng và đồng bộ hóa tài liệu từ yêu cầu cư trú.
    /// (Bao gồm logic Document Reconciliation).
    /// </summary>
    void UpdateUserFromRequest(NguoiDung user, YeuCauCuTru request);

    /// <summary>
    /// Kiểm tra và tạo quan hệ cư trú mới.
    /// </summary>
    /// <summary>
    /// Kiểm tra xem có thể cập nhật cấu trúc hoặc xóa căn hộ không (dựa trên việc có cư dân hay không).
    /// </summary>
    Result CheckCanUpdateOrDeleteCanHo(CanHo canHo, IEnumerable<QuanHeCuTru> currentResidents);

    Result<QuanHeCuTru> CreateRelation(
        int canHoId,
        int userId,
        LoaiQuanHeCuTru loaiQuanHe,
        DateTime startDate,
        IEnumerable<QuanHeCuTru> existingRelations);
}
