namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record TangInToaNhaReadModel
{
    public int TangUid { get; init; }
    public string MaTang { get; init; } = string.Empty;
    public string TenTang { get; init; } = string.Empty;
    public int LoaiTangId { get; init; }
}
