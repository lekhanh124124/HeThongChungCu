namespace HeThongChungCu.Application.Features.QLDoiTac.DTOs;

public class HopDongRequestDto
{
    public int? Id { get; set; }
    public string SoHopDong { get; set; } = string.Empty;
    public DateTimeOffset NgayKy { get; set; }
    public DateTimeOffset NgayHetHan { get; set; }
    public decimal GiaTri { get; set; }
    public string? NoiDung { get; set; }
    public List<int>? TepFileIds { get; set; }

    // Dịch vụ của hợp đồng
    public string MaDichVu { get; set; } = string.Empty;
    public string TenDichVu { get; set; } = string.Empty;
    public int LoaiDichVuId { get; set; }
    public string DonViTinh { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public bool IsBatBuoc { get; set; }
    public int? IconId { get; set; }
    public int? SoLuongToiDa { get; set; }
}

