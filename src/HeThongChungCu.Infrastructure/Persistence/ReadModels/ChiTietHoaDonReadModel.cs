namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

public class ChiTietHoaDonReadModel
{
    public int Id { get; set; }
    public int HoaDonId { get; set; }
    public int LoaiChiTietHoaDonId { get; set; }
    public string TenMucPhi { get; set; } = null!;
    public decimal SoLuong { get; set; }
    public decimal DonGia { get; set; }
    public decimal ThanhTien { get; set; }
    public int? LoaiDinhGiaId { get; set; }
    public string? GhiChu { get; set; }
}
