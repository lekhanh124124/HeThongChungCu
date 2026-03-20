namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal sealed class GetListPhuongTienReadModel
{
    public int TotalCount { get; init; }

    public int Id { get; init; }
    public int CanHoId { get; init; }
    public string MaCanHo { get; init; } = string.Empty;
    public string MaTang { get; init; } = string.Empty;
    public string MaToaNha { get; init; } = string.Empty;

    public string TenPhuongTien { get; init; } = string.Empty;
    public int LoaiPhuongTienId { get; init; }
    public string BienSo { get; init; } = string.Empty;
    public string MauXe { get; init; } = string.Empty;
    public int TrangThaiPhuongTienId { get; init; }
}
