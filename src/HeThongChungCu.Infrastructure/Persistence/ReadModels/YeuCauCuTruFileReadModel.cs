namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record YeuCauCuTruFileReadModel
{
    public int Id { get; init; }
    public string FileUrl { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public int DocumentId { get; init; }
}
