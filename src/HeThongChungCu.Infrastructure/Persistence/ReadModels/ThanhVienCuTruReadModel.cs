namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal sealed class ThanhVienCuTruReadModel
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public int LoaiQuanHeCuTruId { get; init; }
    public DateTime NgayBatDau { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? AnhDaiDienUrl { get; init; }
}
