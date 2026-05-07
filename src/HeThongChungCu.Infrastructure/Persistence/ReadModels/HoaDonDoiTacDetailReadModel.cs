namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record HoaDonDoiTacDetailReadModel
{
    public int Id { get; init; }
    public int HopDongDoiTacId { get; init; }
    public string SoHopDong { get; init; } = null!;
    public int DoiTacId { get; init; }
    public string TenDoiTac { get; init; } = null!;
    public string? TenCongTy { get; init; }
    public string? NguoiDaiDienDoiTac { get; init; }
    public string? SoDienThoaiDoiTac { get; init; }
    public string? EmailDoiTac { get; init; }
    public string? NoiDungHopDong { get; init; }
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
    public DateTimeOffset CreatedAt { get; init; }
    public int CreatedBy { get; init; }
    public string? TenNguoiTao { get; init; }
    public DateTimeOffset? ModifiedAt { get; init; }
    public int? ModifiedBy { get; init; }
    public string? TenNguoiSua { get; init; }
}
