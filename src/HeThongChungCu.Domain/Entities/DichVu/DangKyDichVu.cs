using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class DangKyDichVu : AggregateRoot
{
    public int CanHoId { get; private set; }
    public int DichVuId { get; private set; }
    public ThoiGianHieuLuc ThoiGian { get; private set; } = null!;
    public int SoLuong { get; private set; }
    public TrangThaiDangKy TrangThaiDangKyId { get; private set; } = null!;

    private DangKyDichVu() { }

    public DangKyDichVu(int canHoId, int dichVuId, DateTimeOffset ngayBatDau, int soLuong = 1)
    {
        CanHoId = canHoId;
        DichVuId = dichVuId;
        ThoiGian = new ThoiGianHieuLuc(ngayBatDau);
        SoLuong = soLuong;
        TrangThaiDangKyId = TrangThaiDangKy.ChoDuyet;
    }

    public void UpdateSoLuong(int soLuong)
    {
        if (soLuong < 0) throw new BusinessException("Số lượng không được âm.");
        SoLuong = soLuong;
    }

    public void HuyDangKy(DateTimeOffset ngayKetThuc)
    {
        ThoiGian = new ThoiGianHieuLuc(ThoiGian.NgayBatDau, ngayKetThuc);
        TrangThaiDangKyId = TrangThaiDangKy.DaHuy;
    }

    public void UpdateStatus(TrangThaiDangKy nextStatus)
    {
        TrangThaiDangKyId = nextStatus;
    }
}
