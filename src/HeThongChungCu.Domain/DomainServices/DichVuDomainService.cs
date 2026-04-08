using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Interfaces;

namespace HeThongChungCu.Domain.DomainServices;

public class DichVuDomainService : IDichVuDomainService
{
    public Result CanRegister(DichVu dichVu, int sumHienTai, int soLuongMoi, DateTimeOffset? ngayDangKy = null, KhungGioDichVu? khungGio = null)
    {
        // 1. Kiểm tra trạng thái dịch vụ
        if (dichVu.TrangThaiId != Enums.TrangThaiDichVu.HoatDong)
        {
            return Result.Failure(DichVuErrors.NotActive(dichVu.TenDichVu));
        }

        // 2. Kiểm tra tính hợp lệ của Khung giờ (nếu có) dựa trên bảng giá hiện tại
        var currentPrice = dichVu.GetCurrentPrice(ngayDangKy?.DateTime ?? DateTime.Now);
        bool isTheoKhungGio = currentPrice?.LoaiDinhGiaId == LoaiDinhGia.TheoKhungGio;

        if (isTheoKhungGio)
        {
            if (khungGio == null || ngayDangKy == null)
            {
                return Result.Failure(DichVuErrors.MissingSlotInfo);
            }

            if (khungGio.DichVuId != dichVu.Id)
            {
                return Result.Failure(DichVuErrors.InvalidSlot);
            }

            // Kiểm tra thứ trong tuần
            if (khungGio.NgayTrongTuan is not null)
            {
                var thuTrongTuan = NgayTrongTuan.FromValue((int)ngayDangKy.Value.DayOfWeek);
                if (khungGio.NgayTrongTuan != thuTrongTuan)
                {
                    return Result.Failure(DichVuErrors.InvalidDayOfWeek(khungGio.TenKhungGio));
                }
            }
        }

        // 3. Kiểm tra số lượng tối đa (Sức chứa)
        if (dichVu.SoLuongToiDa.HasValue)
        {
            if (sumHienTai + soLuongMoi > dichVu.SoLuongToiDa.Value)
            {
                return Result.Failure(DichVuErrors.CapacityExceeded(
                    dichVu.TenDichVu, 
                    sumHienTai, 
                    dichVu.SoLuongToiDa.Value));
            }
        }

        return Result.Success();
    }
}

