namespace HeThongChungCu.Application.Features.QLDichVu.DTOs;

public class DangKyDichVuResponse
{
    public int Id { get; set; }
    public int CanHoId { get; set; }
    public int DichVuId { get; set; }
    public string MaDichVu { get; set; } = string.Empty;
    public string TenDichVu { get; set; } = string.Empty;
    public int LoaiDichVuId { get; set; }
    public string LoaiDichVuTen { get; set; } = string.Empty;
    public int SoLuong { get; set; }
    public DateTimeOffset NgayBatDau { get; set; }
    public DateTimeOffset? NgayKetThuc { get; set; }
    public int TrangThaiDangKyId { get; set; }
    public string TrangThaiDangKyTen { get; set; } = string.Empty;
}
