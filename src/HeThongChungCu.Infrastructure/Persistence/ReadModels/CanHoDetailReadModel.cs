namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record CanHoDetailReadModel
{
    public int Id { get; init; }
    public int TangId { get; init; }
    public string TenTang { get; init; } = string.Empty;
    public string MaCanHo { get; init; } = string.Empty;
    public string TenCanHo { get; init; } = string.Empty;
    public decimal DienTich { get; init; }
    public int SoPhongNgu { get; init; }
    public int SoPhongTam { get; init; }
    public int LoaiCanHoId { get; init; }
    public int TinhTrangCanHoId { get; init; }
}
