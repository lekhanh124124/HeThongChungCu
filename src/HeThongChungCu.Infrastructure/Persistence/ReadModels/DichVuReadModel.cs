namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record DichVuReadModel
{
    public int TotalCount { get; init; }
    public int Id { get; init; }
    public string MaDichVu { get; init; } = string.Empty;
    public string TenDichVu { get; init; } = string.Empty;
    public int LoaiDichVuId { get; init; }
    public string DonViTinh { get; init; } = string.Empty;
    public string? MoTa { get; init; }
    public bool IsBatBuoc { get; init; }
    public int TrangThaiId { get; init; }
    public string? IconUrl { get; init; }
}
