using System;

namespace HeThongChungCu.Application.Features.QLPhanAnh.DTOs;

public class PhanAnhResponse
{
    public int Id { get; set; }
    public int CanHoId { get; set; }
    public string TenCanHo { get; set; } = string.Empty;
    public string TieuDe { get; set; } = string.Empty;
    public int LoaiPhanAnhId { get; set; }
    public string LoaiPhanAnhTen { get; set; } = string.Empty;
    public int TrangThaiPhanAnhId { get; set; }
    public string TrangThaiPhanAnhTen { get; set; } = string.Empty;
    public int? NguoiXuLyId { get; set; }
    public string? TenNguoiXuLy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public string TenNguoiGui { get; set; } = string.Empty;
}
