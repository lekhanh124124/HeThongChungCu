using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class ThietBi : AggregateRoot
{
    public string MaThietBi { get; private set; } = null!;
    public string TenThietBi { get; private set; } = null!;
    public string LoaiThietBi { get; private set; } = null!;
    public string ViTri { get; private set; } = null!;
    public DateTimeOffset NgayMua { get; private set; }
    public DateTimeOffset? NgayHetHanBaoHanh { get; private set; }
    public decimal? GiaTriBanDau { get; private set; }
    public TrangThaiThietBi TrangThaiThietBiId { get; private set; } = null!;
    public string? GhiChu { get; private set; }

    private ThietBi() : base() { } // EF Core

    private ThietBi(
        string maThietBi,
        string tenThietBi,
        string loaiThietBi,
        string viTri,
        DateTimeOffset ngayMua,
        DateTimeOffset? ngayHetHanBaoHanh,
        decimal? giaTriBanDau,
        TrangThaiThietBi trangThaiThietBiId,
        string? ghiChu) : base()
    {
        MaThietBi = maThietBi;
        TenThietBi = tenThietBi;
        LoaiThietBi = loaiThietBi;
        ViTri = viTri;
        NgayMua = ngayMua;
        NgayHetHanBaoHanh = ngayHetHanBaoHanh;
        GiaTriBanDau = giaTriBanDau;
        TrangThaiThietBiId = trangThaiThietBiId;
        GhiChu = ghiChu;
    }

    public static ThietBi Create(
        string maThietBi,
        string tenThietBi,
        string loaiThietBi,
        string viTri,
        DateTimeOffset ngayMua,
        DateTimeOffset? ngayHetHanBaoHanh,
        decimal? giaTriBanDau,
        string? ghiChu)
    {
        return new ThietBi(
            maThietBi,
            tenThietBi,
            loaiThietBi,
            viTri,
            ngayMua,
            ngayHetHanBaoHanh,
            giaTriBanDau,
            TrangThaiThietBi.HoatDongTot,
            ghiChu);
    }

    public void Update(
        string tenThietBi,
        string loaiThietBi,
        string viTri,
        DateTimeOffset ngayMua,
        DateTimeOffset? ngayHetHanBaoHanh,
        decimal? giaTriBanDau,
        string? ghiChu)
    {
        TenThietBi = tenThietBi;
        LoaiThietBi = loaiThietBi;
        ViTri = viTri;
        NgayMua = ngayMua;
        NgayHetHanBaoHanh = ngayHetHanBaoHanh;
        GiaTriBanDau = giaTriBanDau;
        GhiChu = ghiChu;
    }

    public void UpdateTrangThai(TrangThaiThietBi trangThaiMoi)
    {
        TrangThaiThietBiId = trangThaiMoi;
    }
}
