using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Interfaces;

public interface ICanHoDomainService
{
    /// <summary>
    /// Kiểm tra xem có thể tạo căn hộ mới tại tầng này không.
    /// </summary>
    Result CanCreateCanHo(Tang tang, string maCanHo, bool isMaExists);

    /// <summary>
    /// Kiểm tra xem có thể cập nhật thông tin cấu trúc căn hộ không.
    /// </summary>
    Result CanUpdateStructure(CanHo canHo, string newMaCanHo, bool isMaExists, bool hasActiveResidents);
}
