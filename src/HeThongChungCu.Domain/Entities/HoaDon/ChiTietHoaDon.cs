using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class ChiTietHoaDon : BaseEntity
{
    public int HoaDonId { get; private set; }
    public LoaiChiTietHoaDon LoaiChiTietId { get; private set; } = null!;
    public int DichVuId { get; private set; }
    public string TenDichVu { get; private set; } = string.Empty;
    public double? ChiSoDau { get; private set; }
    public double? ChiSoCuoi { get; private set; }
    public double SoLuong { get; private set; }
    public decimal DonGia { get; private set; }
    public decimal ThanhTien { get; private set; }
    public string GhiChu { get; private set; } = string.Empty;

    private ChiTietHoaDon() { } // EF Core

    internal ChiTietHoaDon(
        int hoaDonId,
        LoaiChiTietHoaDon loaiChiTietId,
        int dichVuId,
        string tenDichVu,
        double soLuong,
        decimal donGia,
        double? chiSoDau = null,
        double? chiSoCuoi = null,
        string ghiChu = "")
    {
        if (chiSoDau.HasValue && chiSoCuoi.HasValue)
        {
            if (chiSoCuoi < chiSoDau)
                throw new BusinessException("Chỉ số cuối phải >= chỉ số đầu.");
        }

        if (soLuong < 0)
            throw new BusinessException("Số lượng không hợp lệ.");

        if (donGia < 0)
            throw new BusinessException("Đơn giá không hợp lệ.");

        HoaDonId = hoaDonId;
        LoaiChiTietId = loaiChiTietId;
        DichVuId = dichVuId;
        TenDichVu = tenDichVu;
        SoLuong = soLuong;
        DonGia = donGia;
        ChiSoDau = chiSoDau;
        ChiSoCuoi = chiSoCuoi;
        GhiChu = ghiChu;
        ThanhTien = (decimal)soLuong * donGia;
    }
}
