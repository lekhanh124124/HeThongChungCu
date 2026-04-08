namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal sealed class NhanVienReadModel
{
    public int TotalCount { get; init; }
    public int Id { get; init; }
    public int NguoiDungId { get; init; }
    public string HoTen { get; init; } = string.Empty;
    public string SoDienThoai { get; init; } = string.Empty;
    public int LoaiNhanVienId { get; init; }
    public int TrangThaiNhanVienId { get; init; }
    public string MaNhanVien { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? AnhDaiDienUrl { get; init; }
    public DateTimeOffset NgayVaoLam { get; init; }
    public DateTimeOffset? NgayNghiLam { get; init; }
    public string? GhiChu { get; init; }
}
