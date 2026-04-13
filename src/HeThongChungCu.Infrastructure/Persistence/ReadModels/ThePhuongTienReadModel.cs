namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record ThePhuongTienReadModel
{
    public int Id { get; init; }
    public int PhuongTienId { get; init; }
    public string MaThe { get; init; } = string.Empty;
    public DateTime? NgayBatDau { get; init; }
    public DateTime? NgayKetThuc { get; init; }
    public int TrangThaiThePhuongTienId { get; init; }
}
