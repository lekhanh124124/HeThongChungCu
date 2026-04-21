namespace HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;

public class NhanSuThiCongResponse
{
    public int Id { get; set; }
    public int? NhanVienId { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string SoCCCD { get; set; } = string.Empty;
    public string? SoDienThoai { get; set; }
    public string? VaiTro { get; set; }
    public string? GhiChu { get; set; }
    public string? LyDoXoa { get; set; }
}
