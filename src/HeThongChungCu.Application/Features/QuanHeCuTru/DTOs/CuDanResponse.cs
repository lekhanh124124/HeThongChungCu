namespace HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;

public class CuDanResponse
{
    public string MaToaNha { get; set; } = string.Empty;
    public string MaTang { get; set; } = string.Empty;
    public string MaCanHo { get; set; } = string.Empty;
    public int QuanHeCuTruId { get; set; }
    public int UserId { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int LoaiQuanHeCuTruId { get; set; }
    public string TenLoaiQuanHeCuTru { get; set; } = string.Empty;
    public DateTime NgayBatDau { get; set; }
    public DateTime? NgayKetThuc { get; set; }
    public bool IsKetThuc { get; set; }
}
