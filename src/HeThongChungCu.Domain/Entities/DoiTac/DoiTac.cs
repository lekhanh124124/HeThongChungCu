using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class DoiTac : AggregateRoot
{
    public string TenDoiTac { get; private set; } = string.Empty;
    public string? TenCongTy { get; private set; }
    public string? NguoiDaiDien { get; private set; }
    public string? SoGiayPhepKD { get; private set; }
    public string? MaSoThue { get; private set; }
    public DiaChi DiaChi { get; private set; } = null!;
    public SoDienThoai? SoDienThoai { get; private set; }
    public Email? Email { get; private set; }
    public DateTimeOffset? NgayKyHopDong { get; private set; }
    public DateTimeOffset? NgayHetHan { get; private set; }
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
        DateTimeOffset? ngayKyHopDong = null,
        DateTimeOffset? ngayHetHan = null)
    {
        if (string.IsNullOrWhiteSpace(tenDoiTac))
            throw new BusinessException("Tên đối tác không được để trống.");

        TenDoiTac = tenDoiTac;
        TenCongTy = tenCongTy;
        NguoiDaiDien = nguoiDaiDien;
        SoGiayPhepKD = soGiayPhepKD;
        MaSoThue = maSoThue;
        DiaChi = new DiaChi(diaChi);
        SoDienThoai = new SoDienThoai(soDienThoai);
        Email = new Email(email);
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
        DateTimeOffset? ngayKyHopDong,
        DateTimeOffset? ngayHetHan)
    {
        if (string.IsNullOrWhiteSpace(tenDoiTac))
            throw new BusinessException("Tên đối tác không được để trống.");

        TenDoiTac = tenDoiTac;
        TenCongTy = tenCongTy;
        NguoiDaiDien = nguoiDaiDien;
        SoGiayPhepKD = soGiayPhepKD;
        MaSoThue = maSoThue;
        DiaChi = new DiaChi(diaChi);
        SoDienThoai = new SoDienThoai(soDienThoai);
        Email = new Email(email);
        NgayKyHopDong = ngayKyHopDong;
        NgayHetHan = ngayHetHan;
    }

    public void UpdateStatus(TrangThaiHopDong nextStatus)
    {
        TrangThaiHopDongId = nextStatus;
    }
}
