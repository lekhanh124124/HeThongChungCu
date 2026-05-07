namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record LichBaoTriReadModel
{
    public int TotalCount { get; init; }
    public int Id { get; init; }
    public int ThietBiId { get; init; }
    public string TenThietBi { get; init; } = string.Empty;
    public string MaThietBi { get; init; } = string.Empty;
    public int HangMucBaoTriId { get; init; }
    public string TenHangMuc { get; init; } = string.Empty;
    public string MaHangMuc { get; init; } = string.Empty;
    public int TanSuatBaoTriId { get; init; }
    public DateTimeOffset NgayBatDau { get; init; }
    public DateTimeOffset? NgayKetThuc { get; init; }
    public DateTimeOffset? NgayBaoTriGanNhat { get; init; }
    public DateTimeOffset NgayBaoTriTiepTheo { get; init; }
    public bool IsActive { get; init; }
}
