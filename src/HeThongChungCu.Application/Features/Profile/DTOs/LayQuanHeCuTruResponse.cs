namespace HeThongChungCu.Application.Features.Profile.DTOs;

public class LayQuanHeCuTruResponse
{
    public int QuanHeCuTruId { get; set; }
    public int CanHoId { get; set; }
    public string MaCanHo { get; set; } = null!;
    public int ToaNhaId { get; set; }
    public string TenToaNha { get; set; } = null!;
    public int LoaiQuanHeCuTruId { get; set; }
    public string LoaiQuanHeTen { get; set; } = null!;
    public DateTime NgayBatDau { get; set; }
    public bool IsKetThuc { get; set; }

    // Additional CanHo info if needed
    public decimal DienTich { get; set; }
    public int Tang { get; set; }
}
