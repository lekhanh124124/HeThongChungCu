using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Interfaces;

public interface IVehicleRegistryService
{
    /// <summary>
    /// Kiểm tra xem việc đăng ký hoặc cập nhật phương tiện có hợp lệ về mặt nghiệp vụ không.
    /// (Quota, tính duy nhất của biển số).
    /// </summary>
    Result CanRegisterOrUpdateVehicle(
        CanHo apartment,
        LoaiPhuongTien targetVehicleType,
        IEnumerable<PhuongTien> activeVehiclesInApartment,
        bool isLicensePlateDuplicate,
        int? editingVehicleId = null);

    /// <summary>
    /// Kích hoạt phương tiện và kiểm tra hạn mức.
    /// </summary>
    Result KichHoatPhuongTien(PhuongTien phuongTien, CanHo apartment, IEnumerable<PhuongTien> activeVehiclesInApartment);
}
