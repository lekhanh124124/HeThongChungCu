namespace HeThongChungCu.Application.Features.PhuongTien.DTOs;

public class PhuongTienResponse
{
    public int Id { get; set; }
    public string MaToaNha { get; set; } = string.Empty;
    public string MaTang { get; set; } = string.Empty;
    public string MaCanHo { get; set; } = string.Empty;
    public string TenPhuongTien { get; set; } = string.Empty;
    public int LoaiPhuongTienId { get; set; } 
    public string TenLoaiPhuongTien { get; set; } = string.Empty;
    public string BienSo { get; set; } = string.Empty;
    public string MauXe { get; set; } = string.Empty;
    public int TrangThaiPhuongTienId { get; set; }
    public string TenTrangThaiPhuongTien { get; set; } = string.Empty;
    public IReadOnlyList<ThePhuongTienResponse> ThePhuongTiens { get; set; } = new List<ThePhuongTienResponse>();
}