namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record DoiTacReadModel
{
    public int TotalCount { get; init; }
    public int Id { get; init; }
    public string TenDoiTac { get; init; } = null!;
    public string? TenCongTy { get; init; }
    public string? NguoiDaiDien { get; init; }
    public string? SoDienThoai { get; init; }
    public string? Email { get; init; }
}
