namespace HeThongChungCu.Application.Features.PhuongTien.DTOs;

public class ThePhuongTienResponse
{
    public int Id { get; set; }
    public int PhuongTienId { get; set; }
    public string MaThe { get; set; } = string.Empty;
    public DateTime? NgayBatDau { get; set; }
    public DateTime? NgayKetThuc { get; set; }
    public bool IsLocked { get; set; }
}