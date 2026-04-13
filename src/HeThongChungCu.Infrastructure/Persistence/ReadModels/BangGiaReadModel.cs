namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record BangGiaReadModel
{
    public int? TotalCount { get; init; }
    public int Id { get; init; }
    public string TenBangGia { get; init; } = string.Empty;
    public DateTimeOffset NgayApDung { get; init; }
    public DateTimeOffset? NgayKetThuc { get; init; }
    public int LoaiDinhGiaId { get; init; }
    public bool IsActive { get; init; }
    public decimal? DonGia { get; init; }
    public int DichVuId { get; init; }
}
