namespace HeThongChungCu.Application.Features.QLThanhToan.DTOs;

public class ChiTietDienTichResponse
{
    public int Id { get; set; }
    public string TenMucPhi { get; set; } = null!;
    public string TenLoaiCanHo { get; set; } = null!;
    public decimal DienTich { get; set; }
    public decimal DonGia { get; set; }
    public decimal ThanhTien { get; set; }
}
