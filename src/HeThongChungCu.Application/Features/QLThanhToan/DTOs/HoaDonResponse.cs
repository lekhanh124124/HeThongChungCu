namespace HeThongChungCu.Application.Features.QLThanhToan.DTOs;

public class HoaDonResponse
{
    public int Id { get; set; }
    public int CanHoId { get; set; }
    public string MaHoaDon { get; set; } = null!;
    public int Thang { get; set; }
    public int Nam { get; set; }
    public DateTimeOffset NgayLap { get; set; }
    public DateTimeOffset NgayHanThanhToan { get; set; }
    public decimal TongTien { get; set; }
    public int TrangThaiHoaDonId { get; set; }
    public string TrangThaiHoaDonTen { get; set; } = null!;
}
