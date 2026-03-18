using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class QuanHeCuTru : AggregateRoot
{
    public int CanHoId { get; private set; }
    public int UserId { get; private set; }
    public LoaiQuanHeCuTru LoaiQuanHeCuTruId { get; private set; } = null!;
    public DateTime NgayBatDau { get; private set; }
    public DateTime? NgayKetThuc { get; private set; }
    public bool IsKetThuc { get; private set; }

    private QuanHeCuTru() { } // EF Core

    public QuanHeCuTru(
        int canHoId, 
        int userId, 
        LoaiQuanHeCuTru loaiQuanHeCuTruId, 
        DateTime ngayBatDau,
        IEnumerable<QuanHeCuTru> existingRelations)
    {
        if (existingRelations.Any(x => x.UserId == userId && !x.IsKetThuc))
            throw new BusinessException("Cư dân này đã đang cư trú tại căn hộ.");

        if (loaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo && existingRelations.Any(x => x.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo && !x.IsKetThuc))
            throw new BusinessException("Căn hộ đã có chủ hộ.");

        CanHoId = canHoId;
        UserId = userId;
        LoaiQuanHeCuTruId = loaiQuanHeCuTruId;
        NgayBatDau = ngayBatDau;
        IsKetThuc = false;
    }

    public void ThayDoiLoaiQuanHe(LoaiQuanHeCuTru loaiQuanHeCuTruId)
    {
        if (IsKetThuc)
            throw new BusinessException($"Quan hệ cư trú này đã kết thúc.");

        LoaiQuanHeCuTruId = loaiQuanHeCuTruId;
    }

    public void KetThucCuTru(DateTime ngayKetThuc)
    {
        if (IsKetThuc)
            throw new BusinessException($"Quan hệ cư trú này đã kết thúc.");

        NgayKetThuc = ngayKetThuc;
        IsKetThuc = true;
    }
}
