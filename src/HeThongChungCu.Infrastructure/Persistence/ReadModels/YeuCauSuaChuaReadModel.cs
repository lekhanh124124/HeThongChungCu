namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

public class YeuCauSuaChuaReadModel
{
    public int TotalCount { get; set; }
    public int Id { get; set; }
    public int CanHoId { get; set; }
    public string? TenCanHo { get; set; }
    public string? NoiDung { get; set; }
    public int TrangThaiSuaChuaId { get; set; }
    public int LoaiSuCoId { get; set; }
    public int? MucDoUuTienChotId { get; set; }
    public int TrangThaiYeuCauId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public int LoaiYeuCauCuDanId { get; set; }
    public string? TenNguoiGui { get; set; }

    // Extension fields for Detail
    public int? PhamViId { get; set; }
    public string? MoTaViTri { get; set; }
    public DateTimeOffset? HenTu { get; set; }
    public DateTimeOffset? HenDen { get; set; }
    public string? KetQuaXuLy { get; set; }
    public string? LyDoHuy { get; set; }
    public decimal? ChiPhiDuKien { get; set; }
    public decimal? ChiPhiThucTe { get; set; }
    public bool? IsMienPhi { get; set; }
    public string? GhiChuBaoGia { get; set; }
    public int? HopDongDoiTacId { get; set; }
    public string? TenDoiTac { get; set; }
    public int? NguoiXuLyId { get; set; }
    public string? TenNguoiXuLy { get; set; }
    public DateTimeOffset? NgayXuLy { get; set; }
    public string? LyDo { get; set; }
}
