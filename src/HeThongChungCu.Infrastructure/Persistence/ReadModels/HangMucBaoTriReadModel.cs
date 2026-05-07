namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record HangMucBaoTriReadModel
{
    public int TotalCount { get; init; }
    public int Id { get; init; }
    public string MaHangMuc { get; init; } = string.Empty;
    public string TenHangMuc { get; init; } = string.Empty;
    public string? MoTa { get; init; }
    public int ThoiGianUocTinhPhut { get; init; }
    public decimal ChiPhiUocTinh { get; init; }
    public string ChecklistTieuChuan { get; init; } = string.Empty;
}
