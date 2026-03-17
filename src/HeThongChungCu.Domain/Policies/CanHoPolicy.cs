using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Policies;

public class CanHoPolicy : ICanHoPolicy
{
    public void ValidateCreate(decimal dienTich, int soPhongNgu, int soPhongTam)
    {
        ValidateStructure(dienTich, soPhongNgu, soPhongTam);
    }

    public void ValidateUpdate(
        CanHo currentCanHo,
        decimal newDienTich,
        int newSoPhongNgu,
        int newSoPhongTam,
        LoaiCanHo newLoaiCanHo,
        bool hasActiveResidents)
    {
        if (hasActiveResidents)
        {
            if (currentCanHo.DienTich != newDienTich ||
                currentCanHo.SoPhongNgu != newSoPhongNgu ||
                currentCanHo.SoPhongTam != newSoPhongTam ||
                currentCanHo.LoaiCanHoId != newLoaiCanHo)
            {
                throw new BusinessException("Không được thay đổi cấu trúc căn hộ khi đang có cư dân cư trú.");
            }
        }

        ValidateStructure(newDienTich, newSoPhongNgu, newSoPhongTam);
    }

    public void ValidateDelete(CanHo canHo, bool hasActiveResidents)
    {
        if (hasActiveResidents)
        {
            throw new BusinessException("Không được xóa căn hộ khi đang có cư dân cư trú.");
        }
    }

    public void ValidateStatusChange(CanHo canHo, TrangThaiCanHo nextStatus)
    {
        var currentStatus = canHo.TinhTrangCanHoId;

        if (currentStatus == nextStatus) return;

        // Rule: ChuaBanGiao -> DangTrong -> CoCuDan
        if (currentStatus == TrangThaiCanHo.ChuaBanGiao && nextStatus == TrangThaiCanHo.CoCuDan)
        {
            throw new BusinessException("Không được chuyển trực tiếp từ 'Chưa bàn giao' sang 'Có cư dân'. Phải qua trạng thái 'Đang trống'.");
        }

        // Rule: CoCuDan -> DangTrong -> ChuaBanGiao
        if (currentStatus == TrangThaiCanHo.CoCuDan && nextStatus == TrangThaiCanHo.DangTrong)
        {
            throw new BusinessException("Không được chuyển trực tiếp từ 'Có cư dân' sang 'Đang trống'. Phải qua trạng thái 'Chưa bàn giao'.");
        }
        // Add more transition rules as needed based on project requirements
    }

    private void ValidateStructure(decimal dienTich, int soPhongNgu, int soPhongTam)
    {
        if (dienTich <= 0)
            throw new BusinessException("Diện tích căn hộ phải lớn hơn 0.");

        if (soPhongNgu < 0)
            throw new BusinessException("Số phòng ngủ không được âm.");

        if (soPhongTam < 0)
            throw new BusinessException("Số phòng tắm không được âm.");
    }
}
