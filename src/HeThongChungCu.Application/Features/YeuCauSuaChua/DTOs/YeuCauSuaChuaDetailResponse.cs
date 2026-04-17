using HeThongChungCu.Application.Features.UploadMedia.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

public class YeuCauSuaChuaDetailResponse : YeuCauSuaChuaResponse
{
    public string? LyDo { get; set; }

    public int? NguoiXuLyId { get; set; }
    public string? TenNguoiXuLy { get; set; }
    public DateTimeOffset? NgayXuLy { get; set; }

    public int? PhamViId { get; set; }
    public string? PhamViTen { get; set; }

    public string? MoTaViTri { get; set; }
    public DateTimeOffset? HenTu { get; set; }
    public DateTimeOffset? HenDen { get; set; }

    public decimal? ChiPhiDuKien { get; set; }
    public decimal? ChiPhiThucTe { get; set; }
    public bool? IsMienPhi { get; set; }
    public string? GhiChuBaoGia { get; set; }
    public string? KetQuaXuLy { get; set; }
    public string? LyDoHuy { get; set; }

    public int? HopDongDoiTacId { get; set; }
    public string? TenDoiTac { get; set; }

    public List<NhanSuSuaChuaResponse> NhanSuSuaChuas { get; set; } = [];
    public List<TepTaiLieuResponse> DanhSachTep { get; set; } = [];
}
