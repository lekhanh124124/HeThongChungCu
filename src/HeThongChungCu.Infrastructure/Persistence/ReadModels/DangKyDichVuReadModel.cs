namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record DangKyDichVuReadModel
{
    public int? TotalCount { get; init; }
    public int Id { get; init; }
    public int CanHoId { get; init; }
    public int DichVuId { get; init; }
    public string MaDichVu { get; init; } = string.Empty;
    public string TenDichVu { get; init; } = string.Empty;
    public int LoaiDichVuId { get; init; }
    public int SoLuong { get; init; }
    public DateTimeOffset NgayBatDau { get; init; }
    public DateTimeOffset? NgayKetThuc { get; init; }
    public int TrangThaiDangKyId { get; init; }
}
