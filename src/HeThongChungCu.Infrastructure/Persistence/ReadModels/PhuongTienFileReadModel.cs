namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record PhuongTienFileReadModel
{
    public int FileId { get; init; }
    public string FileUrl { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
}
