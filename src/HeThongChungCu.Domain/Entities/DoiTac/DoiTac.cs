using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Entities;

public class DoiTac : AggregateRoot
{
    public string TenDoiTac { get; private set; } = string.Empty;
    public string? TenCongTy { get; private set; }
    public string? NguoiDaiDien { get; private set; }
    public string? SoGiayPhepKD { get; private set; }
    public string? MaSoThue { get; private set; }
    public string? DiaChi { get; private set; }
    public string? SoDienThoai { get; private set; }
    public string? Email { get; private set; }
    public DateTime? NgayKyHopDong { get; private set; }
    public DateTime? NgayHetHan { get; private set; }
    public TrangThaiHopDong TrangThaiHopDongId { get; private set; } = null!;

    private DoiTac() { } // EF Core

    public DoiTac(
        string tenDoiTac,
        string? tenCongTy = null,
        string? nguoiDaiDien = null,
        string? soGiayPhepKD = null,
        string? maSoThue = null,
        string? diaChi = null,
        string? soDienThoai = null,
        string? email = null,
        DateTime? ngayKyHopDong = null,
        DateTime? ngayHetHan = null)
    {
        if (string.IsNullOrWhiteSpace(tenDoiTac))
            throw new BusinessException("Tên đối tác không được để trống.");

        TenDoiTac = tenDoiTac;
        TenCongTy = tenCongTy;
        NguoiDaiDien = nguoiDaiDien;
        SoGiayPhepKD = soGiayPhepKD;
        MaSoThue = maSoThue;
        DiaChi = diaChi;
        SoDienThoai = soDienThoai;
        Email = email;
        NgayKyHopDong = ngayKyHopDong;
        NgayHetHan = ngayHetHan;
        TrangThaiHopDongId = TrangThaiHopDong.ChuaKy;
    }

    public void UpdateInfo(
        string tenDoiTac,
        string? tenCongTy,
        string? nguoiDaiDien,
        string? soGiayPhepKD,
        string? maSoThue,
        string? diaChi,
        string? soDienThoai,
        string? email,
        DateTime? ngayKyHopDong,
        DateTime? ngayHetHan)
    {
        if (string.IsNullOrWhiteSpace(tenDoiTac))
            throw new BusinessException("Tên đối tác không được để trống.");

        TenDoiTac = tenDoiTac;
        TenCongTy = tenCongTy;
        NguoiDaiDien = nguoiDaiDien;
        SoGiayPhepKD = soGiayPhepKD;
        MaSoThue = maSoThue;
        DiaChi = diaChi;
        SoDienThoai = soDienThoai;
        Email = email;
        NgayKyHopDong = ngayKyHopDong;
        NgayHetHan = ngayHetHan;
    }

    public void UpdateStatus(TrangThaiHopDong nextStatus)
    {
        TrangThaiHopDongId = nextStatus;
    }
}
