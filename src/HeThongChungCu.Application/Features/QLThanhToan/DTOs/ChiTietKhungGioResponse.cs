namespace HeThongChungCu.Application.Features.QLThanhToan.DTOs;

public class ChiTietKhungGioResponse
{
    public int Id { get; set; }
    public string TenMucPhi { get; set; } = null!;
    public decimal ThanhTien { get; set; }
    public List<ChiTietGiaKhungGioItemResponse> KhungGios { get; set; } = [];
}

public class ChiTietGiaKhungGioItemResponse
{
    public string TenKhungGio { get; set; } = null!;
    public string GioBatDau { get; set; } = null!;
    public string GioKetThuc { get; set; } = null!;
    public decimal DonGia { get; set; }
}
