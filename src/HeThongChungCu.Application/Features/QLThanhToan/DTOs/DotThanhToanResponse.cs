namespace HeThongChungCu.Application.Features.QLThanhToan.DTOs;

public class DotThanhToanResponse
{
    public int Id { get; set; }
    public string TenDot { get; set; } = null!;
    public int Thang { get; set; }
    public int Nam { get; set; }
    public int TrangThaiDotThanhToanId { get; set; }
    public string TrangThaiDotThanhToanTen { get; set; } = null!;
    public DateTimeOffset? NgayPhatHanh { get; set; }
    public string? GhiChu { get; set; }
}
