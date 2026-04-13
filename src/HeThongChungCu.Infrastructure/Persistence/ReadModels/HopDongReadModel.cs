namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record HopDongReadModel
{
    // HopDong fields
    public int HopDongUid { get; init; }
    public string SoHopDong { get; init; } = null!;
    public DateTimeOffset NgayKy { get; init; }
    public DateTimeOffset NgayHetHan { get; init; }
    public decimal GiaTriHopDong { get; init; }
    public string? NoiDung { get; init; }
    public int HopDongDichVuId { get; init; }
    public int TrangThaiHopDongId { get; init; }

    // DichVu fields
    public int DichVuUid { get; init; }
    public string MaDichVu { get; init; } = null!;
    public string TenDichVu { get; init; } = null!;
    public int LoaiDichVuId { get; init; }
    public string DonViTinh { get; init; } = null!;
    public bool IsBatBuoc { get; init; }
    public int DichVuTrangThaiId { get; init; }
}
