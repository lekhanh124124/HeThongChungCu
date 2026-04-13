namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record TangDetailReadModel
{
    public int Id { get; init; }
    public string MaTang { get; init; } = string.Empty;
    public string TenTang { get; init; } = string.Empty;
    public int LoaiTangId { get; init; }
    public int ToaNhaId { get; init; }
    public string TenToaNha { get; init; } = string.Empty;
}
