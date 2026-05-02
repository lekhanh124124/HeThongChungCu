namespace HeThongChungCu.Application.Features.QLThanhToan.DTOs;

public class ChiTietHoaDonResponse
{
    public int Id { get; set; }
    public int LoaiChiTietHoaDonId { get; set; }
    public string LoaiChiTietHoaDonTen { get; set; } = null!;
    public string TenMucPhi { get; set; } = null!;
    public decimal SoLuong { get; set; }
    public decimal DonGia { get; set; }
    public decimal ThanhTien { get; set; }
    public string? GhiChu { get; set; }
}
