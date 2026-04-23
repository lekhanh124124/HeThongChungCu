namespace HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;

public class NhanSuThiCongRequest
{
    public int? Id { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string SoCCCD { get; set; } = string.Empty;
    public string? SoDienThoai { get; set; }
    public string? VaiTro { get; set; }
    public string? GhiChu { get; set; }
}
