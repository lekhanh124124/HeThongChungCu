namespace HeThongChungCu.Application.Features.QLCuTru.DTOs;

public record YeuCauCuTruResponse
{
    public int Id { get; init; }
    public int CanHoId { get; init; }
    public string MaCanHo { get; init; } = null!;
    public int LoaiYeuCauId { get; init; }
    public string TenLoaiYeuCau { get; init; } = null!;
    public int? QuanHeCuTruId { get; init; }
    
    public string? ProposedFirstName { get; init; }
    public string? ProposedLastName { get; init; }
    public DateTime? ProposedDob { get; init; }
    public int? ProposedGioiTinhId { get; init; }
    public string? ProposedPhoneNumber { get; init; }
    public int? ProposedLoaiQuanHeId { get; init; }
    
    public string? NoiDung { get; init; }
    public string? Reason { get; init; }
    public int TrangThaiId { get; init; }
    public string TenTrangThai { get; init; } = null!;
    
    public DateTimeOffset CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
    public int? ProcessedBy { get; init; }
    
    public List<TaiLieuResponse> Documents { get; init; } = [];
}
