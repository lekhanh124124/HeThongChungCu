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
    public DateTime NgayVaoLam { get; init; }
    public DateTime? NgayNghiLam { get; init; }
    public string? GhiChu { get; init; }
}
