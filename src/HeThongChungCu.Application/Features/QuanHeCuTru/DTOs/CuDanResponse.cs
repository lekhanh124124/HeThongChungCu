namespace HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;

public class CuDanResponse
{
    public int QuanHeCuTruId { get; set; }
    public int UserId { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int LoaiQuanHeCuTruId { get; set; }
    public string TenLoaiQuanHeCuTru { get; set; } = string.Empty;
    public DateTime NgayBatDau { get; set; }
}
