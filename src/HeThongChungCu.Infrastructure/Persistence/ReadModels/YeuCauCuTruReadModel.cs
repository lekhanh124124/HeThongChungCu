namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

public record YeuCauCuTruReadModel
{
    public int TotalCount { get; init; }
    public int Id { get; init; }
    public int CanHoId { get; init; }
    public int LoaiYeuCauId { get; init; }
    public int TrangThaiId { get; init; }
    public string? Reason { get; init; }
    public string? NoiDung { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    // Flattened fields
    public int? ProposedLoaiQuanHeId { get; init; }
    public int? QuanHeCuTruId { get; init; }
}
