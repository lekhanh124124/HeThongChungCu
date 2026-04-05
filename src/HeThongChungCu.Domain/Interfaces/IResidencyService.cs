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
    /// Kiểm tra xem người dùng có quyền (Chủ hộ hoặc Người thuê đại diện) để thực hiện yêu cầu cho căn hộ này không.
    /// </summary>
    Result CheckHeadPermission(QuanHeCuTru? requesterRelation);

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
        DateTimeOffset startDate,
        IEnumerable<QuanHeCuTru> existingRelations);

    /// <summary>
    /// Cập nhật trạng thái căn hộ dựa trên số lượng cư dân hiện tại đang cư trú.
    /// </summary>
    void UpdateApartmentStatus(CanHo canHo, IEnumerable<QuanHeCuTru> activeRelations);

    /// <summary>
    /// Bắt đầu cư trú: Cập nhật trạng thái căn hộ khi có cư dân mới.
    /// </summary>
    void StartResidency(CanHo canHo, QuanHeCuTru relation, IEnumerable<QuanHeCuTru> allRelations);

    /// <summary>
    /// Kết thúc cư trú: Kết thúc quan hệ và cập nhật lại trạng thái căn hộ nếu cần.
    /// </summary>
    void EndResidency(CanHo canHo, QuanHeCuTru relation, IEnumerable<QuanHeCuTru> allRelations, DateTimeOffset endDate);
}
