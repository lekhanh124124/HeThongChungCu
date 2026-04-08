using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLDichVu.DTOs;

public class DichVuResponse
{
    public int Id { get; init; }
    public string MaDichVu { get; init; } = string.Empty;
    public string TenDichVu { get; init; } = string.Empty;
    public int LoaiDichVuId { get; init; }
    public string LoaiDichVuTen { get; init; } = string.Empty;
    public string DonViTinh { get; init; } = string.Empty;
    public string? MoTa { get; init; }
    public bool IsBatBuoc { get; init; }
    public int? SoLuongToiDa { get; init; }
    public int TrangThaiDichVuId { get; init; }
    public string TrangThaiDichVuTen { get; init; } = string.Empty;
    public string? IconUrl { get; init; }
}

