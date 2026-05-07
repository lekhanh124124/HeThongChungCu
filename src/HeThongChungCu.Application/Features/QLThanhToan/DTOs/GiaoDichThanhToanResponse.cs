namespace HeThongChungCu.Application.Features.QLThanhToan.DTOs;

public class GiaoDichThanhToanChiTietResponse
{
    public int ChiTietHoaDonId { get; set; }
    public string TenMucPhi { get; set; } = null!;
    public decimal SoTienPhanBo { get; set; }
}

public class GiaoDichThanhToanResponse
{
    public int Id { get; set; }
    public int HoaDonId { get; set; }
    public decimal SoTien { get; set; }
    public int PhuongThucThanhToanId { get; set; }
    public DateTimeOffset NgayGiaoDich { get; set; }
    public string? MaGiaoDich { get; set; }
    public string? GhiChu { get; set; }

    public List<GiaoDichThanhToanChiTietResponse> ChiTiet { get; set; } = new();
}
