namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record DoiTacContractFileReadModel
{
    public int FileUid { get; init; }
    public string FileUrl { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public int HopDongDoiTacId { get; init; }
}
