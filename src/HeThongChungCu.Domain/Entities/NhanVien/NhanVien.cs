using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class NhanVien : AggregateRoot
{
    public int NguoiDungId { get; private set; }
    public LoaiNhanVien LoaiNhanVienId { get; private set; } = null!;
    public TrangThaiNhanVien TrangThaiNhanVienId { get; private set; } = null!;
    public string MaNhanVien { get; private set; } = string.Empty;
    public DateTime NgayVaoLam { get; private set; }
    public DateTime? NgayNghiLam { get; private set; }
    public string? GhiChu { get; private set; }

    private NhanVien() { } // EF Core

    public NhanVien(int nguoiDungId, LoaiNhanVien loaiNhanVien, string maNhanVien, DateTime ngayVaoLam, string? ghiChu = null)
    {
        NguoiDungId = nguoiDungId;
        LoaiNhanVienId = loaiNhanVien;
        TrangThaiNhanVienId = TrangThaiNhanVien.DangLamViec;
        MaNhanVien = maNhanVien;
        NgayVaoLam = ngayVaoLam;
        GhiChu = ghiChu;
    }

    public void UpdateProfile(LoaiNhanVien loaiNhanVien, DateTime ngayVaoLam, string? ghiChu = null)
    {
        LoaiNhanVienId = loaiNhanVien;
        NgayVaoLam = ngayVaoLam;
        GhiChu = ghiChu;
    }

    public void CapNhatTrangThai(TrangThaiNhanVien trangThai, DateTime currentDate)
    {
        TrangThaiNhanVienId = trangThai;
        if (trangThai == TrangThaiNhanVien.DaNghiViec)
        {
            NgayNghiLam = currentDate;
        }
        else
        {
            NgayNghiLam = null;
        }
    }
}
