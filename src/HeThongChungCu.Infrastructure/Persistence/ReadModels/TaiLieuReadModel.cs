namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record TaiLieuReadModel
{
    public int DocId { get; init; }
    public int LoaiGiayToId { get; init; }
    public string SoGiayTo { get; init; } = string.Empty;
    public DateTimeOffset? NgayPhatHanh { get; init; }
    public int FileId { get; init; }
    public string FileUrl { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
}
