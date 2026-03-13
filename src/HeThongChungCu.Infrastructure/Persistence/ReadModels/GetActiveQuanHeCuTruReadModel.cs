namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal sealed class GetActiveQuanHeCuTruReadModel
{
    public int QuanHeCuTruId { get; init; }
    public int CanHoId { get; init; }
    public string MaCanHo { get; init; } = string.Empty;
    public int ToaNhaId { get; init; }
    public string TenToaNha { get; init; } = string.Empty;
    public int LoaiQuanHeCuTruId { get; init; }
    public DateTime NgayBatDau { get; init; }
    public bool IsKetThuc { get; init; }
    public decimal DienTich { get; init; }
    public int Tang { get; init; }
}
