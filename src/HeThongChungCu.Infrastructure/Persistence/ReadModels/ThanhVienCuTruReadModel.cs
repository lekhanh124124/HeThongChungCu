namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record ThanhVienCuTruReadModel
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public int LoaiQuanHeCuTruId { get; init; }
    public DateTimeOffset NgayBatDau { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? AnhDaiDienUrl { get; init; }
}
