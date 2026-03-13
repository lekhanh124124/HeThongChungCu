namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal sealed class GetCauTrucChungCuReadModel
{
    public int ToaNhaId { get; init; }
    public string MaToaNha { get; init; } = string.Empty;
    public string TenToaNha { get; init; } = string.Empty;
    public int ToaNhaTrangThaiId { get; init; }
    public int? TangId { get; init; }
    public string? MaTang { get; init; }
    public string? TenTang { get; init; }
    public int? CanHoId { get; init; }
    public string? MaCanHo { get; init; }
    public int? CanHoTrangThaiId { get; init; }
}
