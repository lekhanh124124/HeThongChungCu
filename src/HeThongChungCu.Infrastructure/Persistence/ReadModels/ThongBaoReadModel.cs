namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record ThongBaoReadModel
{
    public int TotalCount { get; init; }
    public int Id { get; init; }
    public int ThongBaoId { get; init; }
    public string TieuDe { get; init; } = string.Empty;
    public string NoiDung { get; init; } = string.Empty;
    public int LoaiThongBaoId { get; init; }
    public string? ReferenceId { get; init; }
    public string? Metadata { get; init; }
    public bool IsRead { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? ReadAt { get; init; }
}
