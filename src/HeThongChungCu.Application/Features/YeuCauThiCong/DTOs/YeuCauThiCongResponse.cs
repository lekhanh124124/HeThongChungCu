namespace HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;

public class YeuCauThiCongResponse
{
    public int Id { get; set; }
    public int CanHoId { get; set; }
    public string TenCanHo { get; set; } = string.Empty;
    public string HangMucThiCong { get; set; } = string.Empty;
    public DateTimeOffset DuKienBatDau { get; set; }
    public DateTimeOffset DuKienKetThuc { get; set; }
    public string? TenDonViThiCong { get; set; }
    
    public int TrangThaiYeuCauId { get; set; }
    public string TrangThaiYeuCauTen { get; set; } = string.Empty;
    
    public int? TrangThaiThiCongId { get; set; }
    public string? TrangThaiThiCongTen { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public string TenNguoiGui { get; set; } = string.Empty;
}
