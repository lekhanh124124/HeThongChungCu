using HeThongChungCu.Application.Features.UploadMedia.DTOs;

namespace HeThongChungCu.Application.Features.QLDoiTac.DTOs;

public class HopDongResponse
{
    public int Id { get; set; }
    public string SoHopDong { get; set; } = string.Empty;
    public DateTimeOffset NgayKy { get; set; }
    public DateTimeOffset NgayHetHan { get; set; }
    public decimal GiaTriHopDong { get; set; }
    public int LoaiDichVuId { get; set; }
    public string TenLoaiDichVu { get; set; } = string.Empty;
    public int TrangThaiHopDongId { get; set; }
    public string TrangThaiHopDongTen { get; set; } = string.Empty;
    public string? NoiDung { get; set; }
    public List<UploadFileResponse> Teps { get; set; } = new();

    // Dich Vu Info 
    public string MaDichVu { get; set; } = string.Empty;
    public string TenDichVu { get; set; } = string.Empty;
    public string DonViTinh { get; set; } = string.Empty;
    public bool IsBatBuoc { get; set; }
    public int TrangThaiDichVuId { get; set; }
    public string TrangThaiDichVuTen { get; set; } = string.Empty;
}
