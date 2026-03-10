namespace HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;

public class LichSuCuTruResponse
{
    public int QuanHeCuTruId { get; set; }
    public int CanHoId { get; set; }
    public string MaCanHo { get; set; } = null!;
    public int ToaNhaId { get; set; }
    public string TenToaNha { get; set; } = null!;
    public int UserId { get; set; }
    public string HoTen { get; set; } = null!;
    public int LoaiQuanHeCuTruId { get; set; }
    public string LoaiQuanHeTen { get; set; } = null!;
    public DateTime NgayBatDau { get; set; }
    public DateTime? NgayKetThuc { get; set; }
    public bool IsKetThuc { get; set; }
}
