namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record CanHoInToaNhaReadModel
{
    public int CanHoId { get; init; }
    public string MaCanHo { get; init; } = string.Empty;
    public string TenCanHo { get; init; } = string.Empty;
    public decimal DienTich { get; init; }
    public int SoPhongNgu { get; init; }
    public int SoPhongTam { get; init; }
    public int LoaiCanHoId { get; init; }
    public int TinhTrangCanHoId { get; init; }
    public int TangUid { get; init; }
}
