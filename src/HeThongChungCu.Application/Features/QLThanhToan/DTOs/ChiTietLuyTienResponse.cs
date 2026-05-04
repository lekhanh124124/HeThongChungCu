namespace HeThongChungCu.Application.Features.QLThanhToan.DTOs;

public class ChiTietLuyTienResponse
{
    public int Id { get; set; }
    public string TenMucPhi { get; set; } = null!;
    public decimal ChiSoCu { get; set; }
    public decimal ChiSoMoi { get; set; }
    public decimal SoLuongTieuThu { get; set; }
    public decimal ThanhTien { get; set; }
    public List<ChiTietGiaLuyTienItemResponse> BacThang { get; set; } = [];
}

public class ChiTietGiaLuyTienItemResponse
{
    public string TenBac { get; set; } = null!;
    public decimal TuSo { get; set; }
    public decimal? DenSo { get; set; }
    public decimal SoLuong { get; set; }
    public decimal DonGia { get; set; }
    public decimal ThanhTien { get; set; }
}
