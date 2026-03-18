namespace HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;

public class LichSuCuTruResponse
{
    // Thông tin chung cư
    public int ToaNhaId { get; set; }
    public string TenToaNha { get; set; } = string.Empty;
    public int TangId { get; set; }
    public string TenTang { get; set; } = string.Empty;
    public int CanHoId { get; set; }
    public string TenCanHo { get; set; } = string.Empty;

    // Thông tin cư trú
    public int QuanHeCuTruId { get; set; }
    public int LoaiQuanHeCuTruId { get; set; }
    public string TenLoaiQuanHeCuTru { get; set; } = string.Empty;
    public DateTime NgayBatDau { get; set; }
    public DateTime? NgayKetThuc { get; set; }
}
