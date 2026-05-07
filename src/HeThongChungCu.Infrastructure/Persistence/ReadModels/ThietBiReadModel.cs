namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record ThietBiReadModel
{
    public int TotalCount { get; init; }
    public int Id { get; init; }
    public string MaThietBi { get; init; } = string.Empty;
    public string TenThietBi { get; init; } = string.Empty;
    public string LoaiThietBi { get; init; } = string.Empty;
    public string ViTri { get; init; } = string.Empty;
    public DateTimeOffset NgayMua { get; init; }
    public DateTimeOffset? NgayHetHanBaoHanh { get; init; }
    public decimal? GiaTriBanDau { get; init; }
    public int TrangThaiThietBiId { get; init; }
    public string? GhiChu { get; init; }
}
