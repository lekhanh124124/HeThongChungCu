using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class DangKyDichVu : AggregateRoot
{
    public int CanHoId { get; private set; }
    public int DichVuId { get; private set; }
    public DateTime NgayBatDau { get; private set; }
    public DateTime? NgayKetThuc { get; private set; }
    public int SoLuong { get; private set; }
    public bool IsActive { get; private set; }

    private DangKyDichVu() { }

    public DangKyDichVu(int canHoId, int dichVuId, DateTime ngayBatDau, int soLuong = 1)
    {
        CanHoId = canHoId;
        DichVuId = dichVuId;
        NgayBatDau = ngayBatDau;
        SoLuong = soLuong;
        IsActive = true;
    }

    public void UpdateSoLuong(int soLuong)
    {
        if (soLuong < 0) throw new BusinessException("Số lượng không được âm.");
        SoLuong = soLuong;
    }

    public void HuyDangKy(DateTime ngayKetThuc)
    {
        if (ngayKetThuc < NgayBatDau)
            throw new BusinessException("Ngày kết thúc không được nhỏ hơn ngày bắt đầu.");
        
        NgayKetThuc = ngayKetThuc;
        IsActive = false;
    }

    public void UpdateStatus(bool isActive) => IsActive = isActive;
}
