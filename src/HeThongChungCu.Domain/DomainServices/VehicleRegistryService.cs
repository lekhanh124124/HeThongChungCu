using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Interfaces;

namespace HeThongChungCu.Domain.DomainServices;

public class VehicleRegistryService : IVehicleRegistryService
{
    public Result CanRegisterOrUpdateVehicle(
        CanHo apartment,
        LoaiPhuongTien targetVehicleType,
        IEnumerable<PhuongTien> activeVehiclesInApartment,
        bool isLicensePlateDuplicate,
        int? editingVehicleId = null)
    {
        // 1. Kiểm tra biển số duy nhất
        if (isLicensePlateDuplicate)
        {
            return Result.Failure(PhuongTienErrors.BienSoExists);
        }

        // 2. Tính toán số lượng xe hiện có của loại này trong căn hộ
        // (Loại trừ bản thân xe đang sửa nếu có)
        var currentCount = activeVehiclesInApartment
            .Count(v => v.LoaiPhuongTienId == targetVehicleType && v.Id != editingVehicleId);

        // 3. Kiểm tra Hạn mức (Quota) từ Policy
        var quota = PhuongTienPolicy.GetQuota(apartment.LoaiCanHoId, targetVehicleType);

        if (currentCount >= quota)
        {
            return Result.Failure(PhuongTienErrors.OverQuota(apartment.LoaiCanHoId, targetVehicleType, quota));
        }

        return Result.Success();
    }

    public Result KichHoatPhuongTien(PhuongTien phuongTien, CanHo apartment, IEnumerable<PhuongTien> activeVehiclesInApartment)
    {
        // 1. Tính toán số lượng xe hiện có của loại này trong căn hộ (Loại trừ bản thân xe đang kích hoạt)
        var currentCount = activeVehiclesInApartment
            .Count(v => v.LoaiPhuongTienId == phuongTien.LoaiPhuongTienId && v.Id != phuongTien.Id);

        // 2. Kiểm tra Hạn mức (Quota) từ Policy
        var quota = PhuongTienPolicy.GetQuota(apartment.LoaiCanHoId, phuongTien.LoaiPhuongTienId);

        if (currentCount >= quota)
        {
            return Result.Failure(PhuongTienErrors.OverQuota(apartment.LoaiCanHoId, phuongTien.LoaiPhuongTienId, quota));
        }

        // 3. Gọi phương thức kích hoạt đơn giản của Entity
        phuongTien.Activate();

        return Result.Success();
    }
}
