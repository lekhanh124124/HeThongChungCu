namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal sealed class GetLichSuCuTruReadModel
{
    public int TotalCount { get; init; }
    public int QuanHeCuTruId { get; init; }
    public int CanHoId { get; init; }
    public string MaCanHo { get; init; } = string.Empty;
    public int ToaNhaId { get; init; }
    public string TenToaNha { get; init; } = string.Empty;
    public int UserId { get; init; }
    public string HoTen { get; init; } = string.Empty;
    public int LoaiQuanHeCuTruId { get; init; }
    public DateTime NgayBatDau { get; init; }
    public DateTime? NgayKetThuc { get; init; }
    public bool IsKetThuc { get; init; }
}
