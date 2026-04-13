namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record NhanVienDetailReadModel
{
    public int Id { get; init; }
    public int NguoiDungId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string HoTen { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? SoDienThoai { get; init; }
    public string? CCCD { get; init; }
    public string? DiaChi { get; init; }
    public DateTimeOffset? Dob { get; init; }
    public int? GioiTinhId { get; init; }
    public string? AnhDaiDienUrl { get; init; }
    public int LoaiNhanVienId { get; init; }
    public int TrangThaiNhanVienId { get; init; }
    public string MaNhanVien { get; init; } = string.Empty;
    public DateTimeOffset NgayVaoLam { get; init; }
    public DateTimeOffset? NgayNghiLam { get; init; }
    public string? GhiChu { get; init; }
}
