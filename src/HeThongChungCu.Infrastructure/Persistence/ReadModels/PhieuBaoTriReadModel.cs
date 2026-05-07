namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record PhieuBaoTriReadModel
{
    public int TotalCount { get; init; }
    public int Id { get; init; }
    public string MaPhieu { get; init; } = string.Empty;
    public int ThietBiId { get; init; }
    public string TenThietBi { get; init; } = string.Empty;
    public string MaThietBi { get; init; } = string.Empty;
    public int HangMucBaoTriId { get; init; }
    public string TenHangMuc { get; init; } = string.Empty;
    public int? LichBaoTriId { get; init; }
    public int? HopDongDoiTacId { get; init; }
    public string? SoHopDong { get; init; }
    public string? TenDoiTac { get; init; }
    public DateTimeOffset NgayLapPhieu { get; init; }
    public DateTimeOffset NgayDuKien { get; init; }
    public DateTimeOffset? NgayThucTe { get; init; }
    public decimal? ChiPhiThucTe { get; init; }
    public int TrangThaiPhieuBaoTriId { get; init; }
    public string? GhiChuXuLy { get; init; }
    public string? LyDoHuy { get; init; }
    public int? NguoiKiemDuyetId { get; init; }
    public string? TenNguoiKiemDuyet { get; init; }
}
