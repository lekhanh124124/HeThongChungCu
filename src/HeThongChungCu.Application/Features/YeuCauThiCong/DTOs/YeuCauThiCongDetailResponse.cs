namespace HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;

public class YeuCauThiCongDetailResponse : YeuCauThiCongResponse
{
    public string? NoiDung { get; set; }
    public string? NguoiDaiDien { get; set; }
    public string? SoDienThoaiDaiDien { get; set; }
    
    public decimal? TienDatCoc { get; set; }
    public bool IsDaThuCoc { get; set; }
    public string? GhiChuThuCoc { get; set; }
    public decimal? TienKhauTru { get; set; }
    public string? LyDoKhauTru { get; set; }
    public bool IsDaHoanCoc { get; set; }
    
    public int? NguoiXuLyId { get; set; }
    public string? TenNguoiXuLy { get; set; }
    public DateTimeOffset? NgayXuLy { get; set; }
    public string? LyDo { get; set; } // Ly do tu choi/tra lai
    
    public List<NhanSuThiCongResponse> NhanSuThiCongs { get; set; } = [];
    public List<TepYeuCauThiCongResponse> DanhSachTep { get; set; } = [];
}
