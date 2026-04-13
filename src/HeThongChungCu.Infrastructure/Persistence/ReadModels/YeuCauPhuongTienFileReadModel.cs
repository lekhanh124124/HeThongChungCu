namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record YeuCauPhuongTienFileReadModel
{
    public int Id { get; init; }
    public string FileUrl { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
}
