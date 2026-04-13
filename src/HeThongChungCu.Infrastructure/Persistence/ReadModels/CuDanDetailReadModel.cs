namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record CuDanDetailReadModel
{
    public int NguoiDungId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public DateTimeOffset? Dob { get; init; }
    public int GioiTinhId { get; init; }
    public string? IdCard { get; init; }
    public string? AnhDaiDienUrl { get; init; }
    public int QuanHeCuTruId { get; init; }
    public int LoaiQuanHeCuTruId { get; init; }
    public DateTimeOffset NgayBatDau { get; init; }
    public DateTimeOffset? NgayKetThuc { get; init; }
    public int TrangThaiCuTruId { get; init; }
    public string? DiaChi { get; init; }
}
