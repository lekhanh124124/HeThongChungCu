using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public abstract class NhanSuYeuCau : AuditableEntity
{
    public int YeuCauId { get; private set; }
    public int? NhanVienId { get; private set; } // Liên kết nếu là nhân viên nội bộ
    public string HoTen { get; private set; } = string.Empty;
    public string SoCCCD { get; private set; } = string.Empty;
    public SoDienThoai? SoDienThoai { get; private set; }
    public string? VaiTro { get; private set; }
    public string? GhiChu { get; private set; }

    protected NhanSuYeuCau() { } // EF Core

    protected NhanSuYeuCau(string hoTen, string soCCCD, string? soDienThoai, string? vaiTro, string? ghiChu = null, int? nhanVienId = null)
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
        GhiChu = ghiChu;
        NhanVienId = nhanVienId;
    }

    internal virtual void UpdateInfo(string hoTen, string soCCCD, string? soDienThoai, string? vaiTro, string? ghiChu = null, int? nhanVienId = null)
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
        GhiChu = ghiChu;
        NhanVienId = nhanVienId;
    }
}
