namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record YeuCauCuTruDocumentReadModel
{
    public int Id { get; init; }
    public int LoaiGiayToId { get; init; }
    public string SoGiayTo { get; init; } = string.Empty;
    public DateTimeOffset? NgayPhatHanh { get; init; }
    public int? TargetTaiLieuCuTruId { get; init; }
}
