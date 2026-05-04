namespace HeThongChungCu.Application.Features.QLThanhToan.DTOs;

public class ChiTietCoDinhResponse
{
    public int Id { get; set; }
    public string TenMucPhi { get; set; } = null!;
    public decimal SoLuong { get; set; }
    public decimal DonGia { get; set; }
    public decimal ThanhTien { get; set; }
    public string? GhiChu { get; set; }
}
