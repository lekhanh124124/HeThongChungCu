using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.Events;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class QuanHeCuTru : AggregateRoot
{
    public int CanHoId { get; private set; }
    public int NguoiDungId { get; private set; }
    public LoaiQuanHeCuTru LoaiQuanHeCuTruId { get; private set; } = null!;
    public ThoiGianHieuLuc ThoiGian { get; private set; } = null!;
    public TrangThaiCuTru TrangThaiCuTruId { get; private set; } = null!;

    private QuanHeCuTru() { } // EF Core

    public QuanHeCuTru(
        int canHoId,
        int nguoiDungId,
        LoaiQuanHeCuTru loaiQuanHeCuTruId,
        DateTimeOffset ngayBatDau)
    {
        CanHoId = canHoId;
        NguoiDungId = nguoiDungId;
        LoaiQuanHeCuTruId = loaiQuanHeCuTruId;
        ThoiGian = new ThoiGianHieuLuc(ngayBatDau);
        TrangThaiCuTruId = TrangThaiCuTru.DangCuTru;
    }

    public void ThayDoiLoaiQuanHe(LoaiQuanHeCuTru loaiQuanHeCuTruId)
    {
        if (TrangThaiCuTruId == TrangThaiCuTru.DaKetThuc)
            throw new BusinessException($"Quan hệ cư trú này đã kết thúc.");

        LoaiQuanHeCuTruId = loaiQuanHeCuTruId;
    }

    public void KetThucCuTru(DateTimeOffset ngayKetThuc)
    {
        if (TrangThaiCuTruId == TrangThaiCuTru.DaKetThuc)
            throw new BusinessException($"Quan hệ cư trú này đã kết thúc.");

        ThoiGian = new ThoiGianHieuLuc(ThoiGian.NgayBatDau, ngayKetThuc);
        TrangThaiCuTruId = TrangThaiCuTru.DaKetThuc;

        AddDomainEvent(new KetThucCuTruEvent(CanHoId, NguoiDungId, LoaiQuanHeCuTruId));
    }
}
