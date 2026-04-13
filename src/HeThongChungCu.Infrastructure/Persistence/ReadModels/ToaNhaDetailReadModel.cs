namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record ToaNhaDetailReadModel
{
    public int Id { get; init; }
    public string MaToaNha { get; init; } = string.Empty;
    public string TenToaNha { get; init; } = string.Empty;
    public string DiaChi { get; init; } = string.Empty;
    public string? MoTa { get; init; }
    public int TrangThaiToaNhaId { get; init; }
    public int SoCanHo { get; init; }
}
