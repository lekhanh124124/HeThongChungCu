namespace HeThongChungCu.Application.Features.CuDan.DTOs;

public class QuanHeCuTruResponse
{
    // Thông tin quan hệ cư trú
    public int Id { get; set; }
    public int LoaiQuanHeCuTruId { get; set; }
    public string LoaiQuanHeTen { get; set; } = string.Empty;

    // Thông tin căn hộ
    public int ToaNhaId { get; set; }
    public string MaToaNha { get; set; } = string.Empty;
    public string TenToaNha { get; set; } = string.Empty;
    public int CanHoId { get; set; }
    public string MaCanHo { get; set; } = string.Empty;
    public string TenCanHo { get; set; } = string.Empty;

    // Thông tin quan hệ liên quan
    public int TongCuDan { get; set; }
}
