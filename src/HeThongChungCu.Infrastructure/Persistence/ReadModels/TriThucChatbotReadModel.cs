namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record TriThucChatbotReadModel
{
    public int TotalCount { get; init; }
    public int Id { get; init; }
    public string TieuDe { get; init; } = string.Empty;
    public string NoiDung { get; init; } = string.Empty;
    public string DanhMuc { get; init; } = string.Empty;
    public int ThuTuHienThi { get; init; }
    public bool IsActive { get; init; }
    public bool IsSynced { get; init; }
    public DateTimeOffset? LastSyncedAt { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? ModifiedAt { get; init; }
    public int CreatedBy { get; init; }
}
