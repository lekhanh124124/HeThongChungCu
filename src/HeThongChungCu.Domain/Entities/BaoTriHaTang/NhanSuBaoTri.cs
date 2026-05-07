using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class NhanSuBaoTri : AuditableEntity
{
    public int PhieuBaoTriId { get; private set; }
    public int? NhanVienId { get; private set; } // Liên kết nếu là nhân viên nội bộ
    public string HoTen { get; private set; } = string.Empty;
    public string SoCCCD { get; private set; } = string.Empty;
    public SoDienThoai? SoDienThoai { get; private set; }
    public string? VaiTro { get; private set; }

    private NhanSuBaoTri() : base() { } // EF Core

    private NhanSuBaoTri(string hoTen, string soCCCD, string? soDienThoai, string? vaiTro, int? nhanVienId = null) : base()
    {
        if (nhanVienId == null)
        {
            if (string.IsNullOrWhiteSpace(hoTen))
                throw new BusinessException("Họ tên nhân sự không được để trống.");

            if (string.IsNullOrWhiteSpace(soCCCD))
                throw new BusinessException("Số CCCD không được để trống.");
        }

        HoTen = hoTen;
        SoCCCD = soCCCD;
        SoDienThoai = string.IsNullOrWhiteSpace(soDienThoai) ? null : new SoDienThoai(soDienThoai);
        VaiTro = vaiTro;
        NhanVienId = nhanVienId;
    }

    public static NhanSuBaoTri Create(string hoTen, string soCCCD, string? soDienThoai, string? vaiTro, int? nhanVienId = null)
    {
        return new NhanSuBaoTri(hoTen, soCCCD, soDienThoai, vaiTro, nhanVienId);
    }

    internal void UpdateInfo(string hoTen, string soCCCD, string? soDienThoai, string? vaiTro, int? nhanVienId = null)
    {
        if (nhanVienId == null)
        {
            if (string.IsNullOrWhiteSpace(hoTen))
                throw new BusinessException("Họ tên nhân sự không được để trống.");

            if (string.IsNullOrWhiteSpace(soCCCD))
                throw new BusinessException("Số CCCD không được để trống.");
        }

        HoTen = hoTen;
        SoCCCD = soCCCD;
        SoDienThoai = string.IsNullOrWhiteSpace(soDienThoai) ? null : new SoDienThoai(soDienThoai);
        VaiTro = vaiTro;
        NhanVienId = nhanVienId;
    }
}
