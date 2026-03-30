namespace HeThongChungCu.Application.Features.QLCuTru.DTOs;

public record YeuCauCuTruResponse
{
    public int Id { get; init; }
    public int CanHoId { get; init; }
    public int LoaiYeuCauId { get; init; }
    public string TenLoaiYeuCau { get; init; } = null!;
    public int? TargetQuanHeCuTruId { get; init; }
    
    public string? YeuCauTen { get; init; }
    public string? YeuCauHo { get; init; }
    public DateTime? YeuCauNgaySinh { get; init; }
    public int? YeuCauGioiTinhId { get; init; }
    public string? YeuCauSoDienThoai { get; init; }
    public int? YeuCauLoaiQuanHeId { get; init; }
    
    public string? NoiDung { get; init; }
    public string? LyDo { get; init; }
    public int TrangThaiId { get; init; }
    public string TenTrangThai { get; init; } = null!;
    
    public DateTimeOffset CreatedAt { get; init; }
    public DateTime? NgayXuLy { get; init; }
    public int? NguoiXuLyId { get; init; }
    
    public List<TaiLieuResponse> Documents { get; init; } = [];
}
