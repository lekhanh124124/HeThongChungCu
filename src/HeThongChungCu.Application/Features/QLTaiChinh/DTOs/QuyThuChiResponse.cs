using System;
using System.Collections.Generic;

namespace HeThongChungCu.Application.Features.QLTaiChinh.DTOs;

public record QuyThuChiResponse
{
    public int Id { get; init; }
    public string MaGiaoDich { get; init; } = string.Empty;
    public int LoaiGiaoDichId { get; init; }
    public string TenLoaiGiaoDich { get; init; } = string.Empty;
    public decimal TongSoTien { get; init; }
    public DateTimeOffset NgayGiaoDich { get; init; }
    public int PhuongThucThanhToanId { get; init; }
    public string TenPhuongThucThanhToan { get; init; } = string.Empty;
    public string NguoiGiaoDich { get; init; } = string.Empty;
    public string? ChungTuGoc { get; init; }
    
    public List<ChiTietQuyThuChiResponse> ChiTiets { get; init; } = new();
}

public record ChiTietQuyThuChiResponse
{
    public int Id { get; init; }
    public decimal SoTien { get; init; }
    public string NhomThongKe { get; init; } = string.Empty; // Holds TenDichVu or NhomThongKe text
    public string? GhiChu { get; init; }
    
    // For Income Details (if needed by UI)
    public int? DichVuId { get; init; }
}
