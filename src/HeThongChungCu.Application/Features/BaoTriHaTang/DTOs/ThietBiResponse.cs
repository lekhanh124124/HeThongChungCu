namespace HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

public class ThietBiResponse
{
    public int Id { get; set; }
    public string MaThietBi { get; set; } = null!;
    public string TenThietBi { get; set; } = null!;
    public string LoaiThietBi { get; set; } = null!;
    public string ViTri { get; set; } = null!;
    public DateTimeOffset NgayMua { get; set; }
    public DateTimeOffset? NgayHetHanBaoHanh { get; set; }
    public decimal? GiaTriBanDau { get; set; }
    public int TrangThaiThietBiId { get; set; }
    public string TenTrangThaiThietBi { get; set; } = null!;
    public string? GhiChu { get; set; }
    public int? ToaNhaId { get; set; }
    public string? TenToaNha { get; set; }
}

public class ThietBiDetailResponse : ThietBiResponse
{
    // Kế thừa toàn bộ trường thông tin chi tiết từ ThietBiResponse
}
