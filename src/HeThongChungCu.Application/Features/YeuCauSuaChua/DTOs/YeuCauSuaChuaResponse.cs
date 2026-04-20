namespace HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

public class YeuCauSuaChuaResponse
{
    public int Id { get; set; }
    public int CanHoId { get; set; }
    public string? TenCanHo { get; set; }
    public string? TenTang { get; set; }
    public string? TenToaNha { get; set; }

    public int LoaiYeuCauCuDanId { get; set; }
    public string LoaiYeuCauCuDanTen { get; set; } = null!;

    public int TrangThaiYeuCauId { get; set; }
    public string TrangThaiYeuCauTen { get; set; } = null!;

    public string? NoiDung { get; set; }

    public int LoaiSuCoId { get; set; }
    public string LoaiSuCoTen { get; set; } = null!;

    public int? TrangThaiSuaChuaId { get; set; }
    public string? TrangThaiSuaChuaTen { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public string? TenNguoiGui { get; set; }
}
