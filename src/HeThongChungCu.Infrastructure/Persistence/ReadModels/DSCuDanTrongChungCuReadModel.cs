namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal sealed class DSCuDanTrongChungCuReadModel
{
    public int TotalCount { get; init; }
    public string MaToaNha { get; init; } = string.Empty;
    public string MaTang { get; init; } = string.Empty;
    public string MaCanHo { get; init; } = string.Empty;
    public int QuanHeCuTruId { get; init; }
    public int NguoiDungId { get; init; }
    public string HoTen { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public int LoaiQuanHeCuTruId { get; init; }
    public DateTime NgayBatDau { get; init; }
    public DateTime? NgayKetThuc { get; init; }
    public int TrangThaiCuTruId { get; init; }
}
