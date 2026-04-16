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

    public int? HopDongDoiTacId { get; init; }
    public string? SoHopDong { get; init; }
    public string? TenDoiTac { get; init; }
    public int? TrangThaiHopDongId { get; init; }
    public string? TrangThaiHopDongTen { get; init; }
    public bool IsDoiTacCungCap => HopDongDoiTacId.HasValue;
    public string NguonCungCapDescription => IsDoiTacCungCap ? "Đối tác cung cấp" : "Chung cư cung cấp";
}

