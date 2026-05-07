namespace HeThongChungCu.Application.Features.QLDoiTac.DTOs;

public class HoaDonDoiTacResponse
{
    public int Id { get; set; }
    public int HopDongDoiTacId { get; set; }
    public string SoHopDong { get; set; } = string.Empty;
    public int DoiTacId { get; set; }
    public string TenDoiTac { get; set; } = string.Empty;
    public int Thang { get; set; }
    public int Nam { get; set; }
    public decimal SoTien { get; set; }
    public DateTimeOffset NgayGhiNhan { get; set; }
    public string? GhiChu { get; set; }
    public int TrangThaiThanhToanId { get; set; }
    public string TrangThaiThanhToanTen { get; set; } = string.Empty;
    public int? FileHoaDonId { get; set; }
    public string? FileHoaDonUrl { get; set; }
    public string? FileHoaDonName { get; set; }
}
