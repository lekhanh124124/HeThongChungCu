namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record PhieuBaoTriChecklistBulkReadModel
{
    public int Id { get; init; }
    public int PhieuBaoTriId { get; init; }
    public string NoiDungChecklist { get; init; } = string.Empty;
    public bool? DatYeuCau { get; init; }
    public string? GhiChuThucTe { get; init; }
    public int? AnhMinhHoaId { get; init; }
}
