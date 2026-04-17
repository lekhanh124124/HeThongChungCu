namespace HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

public record NhanSuSuaChuaResponse
{
    public int Id { get; set; }
    public int? NhanVienId { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string SoCCCD { get; set; } = string.Empty;
    public string? SoDienThoai { get; set; } = string.Empty;
    public string? VaiTro { get; set; } = string.Empty;
    public string? GhiChu { get; set; } = string.Empty;
}
