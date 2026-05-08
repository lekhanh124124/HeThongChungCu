using System;

namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

public class YeuCauPhanAnhReadModel
{
    // Common Info
    public int Id { get; set; }
    public int CanHoId { get; set; }
    public string TenCanHo { get; set; } = string.Empty;
    public string TieuDe { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
    public int LoaiPhanAnhId { get; set; }
    public int TrangThaiPhanAnhId { get; set; }
    public int? NguoiXuLyId { get; set; }
    public string? TenNguoiXuLy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public string TenNguoiGui { get; set; } = string.Empty;
    public int LoaiYeuCauCuDanId { get; set; }

    // Rating
    public int? DiemDanhGia { get; set; }
    public string? NhanXetDanhGia { get; set; }
    public DateTimeOffset? NgayDanhGia { get; set; }

    public int TotalCount { get; set; }
}
