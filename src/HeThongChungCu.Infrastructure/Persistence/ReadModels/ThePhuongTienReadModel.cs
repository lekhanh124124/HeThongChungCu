namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record ThePhuongTienReadModel
{
    public int Id { get; init; }
    public int PhuongTienId { get; init; }
    public string MaThe { get; init; } = string.Empty;
    public DateTimeOffset? NgayBatDau { get; init; }
    public DateTimeOffset? NgayKetThuc { get; init; }
    public int TrangThaiThePhuongTienId { get; init; }
}
