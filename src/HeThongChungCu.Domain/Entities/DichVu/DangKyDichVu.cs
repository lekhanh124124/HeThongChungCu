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

    public DangKyDichVu(int canHoId, int dichVuId, DateTimeOffset ngaySuDung, int soLuong = 1, KhungGioDichVu? khungGio = null)
    {
        CanHoId = canHoId;
        DichVuId = dichVuId;
        
        if (khungGio != null)
        {
            var batDau = ngaySuDung.Date.Add(khungGio.GioBatDau);
            var ketThuc = ngaySuDung.Date.Add(khungGio.GioKetThuc);
            ThoiGian = new ThoiGianHieuLuc(
                new DateTimeOffset(batDau, ngaySuDung.Offset), 
                new DateTimeOffset(ketThuc, ngaySuDung.Offset));
        }
        else
        {
            ThoiGian = new ThoiGianHieuLuc(ngaySuDung);
        }

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
