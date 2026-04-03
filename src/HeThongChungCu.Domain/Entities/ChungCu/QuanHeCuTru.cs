using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.Events;

namespace HeThongChungCu.Domain.Entities;

public class QuanHeCuTru : AggregateRoot
{
    public int CanHoId { get; private set; }
    public int NguoiDungId { get; private set; }
    public LoaiQuanHeCuTru LoaiQuanHeCuTruId { get; private set; } = null!;
    public DateTime NgayBatDau { get; private set; }
    public DateTime? NgayKetThuc { get; private set; }
    public TrangThaiCuTru TrangThaiCuTruId { get; private set; } = null!;

    private QuanHeCuTru() { } // EF Core

    public QuanHeCuTru(
        int canHoId,
        int nguoiDungId,
        LoaiQuanHeCuTru loaiQuanHeCuTruId,
        DateTime ngayBatDau)
    {
        CanHoId = canHoId;
        NguoiDungId = nguoiDungId;
        LoaiQuanHeCuTruId = loaiQuanHeCuTruId;
        NgayBatDau = ngayBatDau;
        TrangThaiCuTruId = TrangThaiCuTru.DangCuTru;
    }

    public void ThayDoiLoaiQuanHe(LoaiQuanHeCuTru loaiQuanHeCuTruId)
    {
        if (TrangThaiCuTruId == TrangThaiCuTru.DaKetThuc)
            throw new BusinessException($"Quan hệ cư trú này đã kết thúc.");

        LoaiQuanHeCuTruId = loaiQuanHeCuTruId;
    }

    public void KetThucCuTru(DateTime ngayKetThuc)
    {
        if (TrangThaiCuTruId == TrangThaiCuTru.DaKetThuc)
            throw new BusinessException($"Quan hệ cư trú này đã kết thúc.");

        NgayKetThuc = ngayKetThuc;
        TrangThaiCuTruId = TrangThaiCuTru.DaKetThuc;

        AddDomainEvent(new KetThucCuTruEvent(CanHoId, LoaiQuanHeCuTruId));
    }
}
