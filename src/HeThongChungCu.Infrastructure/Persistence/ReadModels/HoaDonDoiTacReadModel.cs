namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record HoaDonDoiTacReadModel
{
    public int TotalCount { get; init; }
    public int Id { get; init; }
    public int HopDongDoiTacId { get; init; }
    public string SoHopDong { get; init; } = null!;
    public int DoiTacId { get; init; }
    public string TenDoiTac { get; init; } = null!;
    public int Thang { get; init; }
    public int Nam { get; init; }
    public decimal SoTien { get; init; }
    public DateTimeOffset NgayGhiNhan { get; init; }
    public string? GhiChu { get; init; }
    public int TrangThaiThanhToanId { get; init; }
    public int? FileHoaDonId { get; init; }
    public string? FileUrl { get; init; }
    public string? FileName { get; init; }
    public string? ContentType { get; init; }
}
