using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;

public class ChiSoResponse
{
    public int Id { get; set; }
    public int CanHoId { get; set; }
    public string MaCanHo { get; set; } = null!;
    public string TenCanHo { get; set; } = null!;
    public int DichVuId { get; set; }
    public string TenDichVu { get; set; } = null!;
    public decimal ChiSoCu { get; set; }
    public decimal ChiSoMoi { get; set; }
    public decimal SoLuong { get; set; }
    public int Thang { get; set; }
    public int Nam { get; set; }
    public DateTimeOffset NgayGhiNhan { get; set; }
    public TrangThaiChiSo TrangThaiChiSoId { get; set; } = null!;
    public string TrangThaiChiSoName => TrangThaiChiSoId.Name;
    public string? MaTraCuu { get; set; }
}
