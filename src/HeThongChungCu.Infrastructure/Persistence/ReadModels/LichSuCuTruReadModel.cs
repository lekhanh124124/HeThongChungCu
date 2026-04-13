namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record LichSuCuTruReadModel
{
    public int TotalCount { get; init; }

    public int ToaNhaId { get; init; }
    public string TenToaNha { get; init; } = string.Empty;
    public int TangId { get; init; }
    public string TenTang { get; init; } = string.Empty;
    public int CanHoId { get; init; }
    public string TenCanHo { get; init; } = string.Empty;

    public int QuanHeCuTruId { get; init; }
    public int LoaiQuanHeCuTruId { get; init; }
    public DateTimeOffset NgayBatDau { get; init; }
    public DateTimeOffset? NgayKetThuc { get; init; }
}
