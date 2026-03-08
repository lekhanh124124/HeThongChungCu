namespace HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;

public class CuDanResponse
{
    public int QuanHeCuTruId { get; set; }
    public int UserId { get; set; }
    public string HoTen { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public int LoaiQuanHeCuTruId { get; set; }
    public string LoaiQuanHeTen { get; set; } = null!;
    public DateTime NgayBatDau { get; set; }
}
