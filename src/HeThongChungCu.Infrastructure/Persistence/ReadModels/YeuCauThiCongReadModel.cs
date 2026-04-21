namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

public class YeuCauThiCongReadModel
{
    // Common Info
    public int Id { get; set; }
    public int CanHoId { get; set; }
    public string TenCanHo { get; set; } = string.Empty;
    public string? NoiDung { get; set; }
    public int TrangThaiYeuCauId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public string TenNguoiGui { get; set; } = string.Empty;
    public int LoaiYeuCauCuDanId { get; set; }

    // Construction Specific
    public string HangMucThiCong { get; set; } = string.Empty;
    public DateTimeOffset DuKienBatDau { get; set; }
    public DateTimeOffset DuKienKetThuc { get; set; }
    public string? TenDonViThiCong { get; set; }
    public string? NguoiDaiDien { get; set; }
    public string? SoDienThoaiDaiDien { get; set; }

    // Financial & Operational
    public int? TrangThaiThiCongId { get; set; }
    public decimal? TienDatCoc { get; set; }
    public bool IsDaThuCoc { get; set; }
    public string? GhiChuThuCoc { get; set; }
    public decimal? TienKhauTru { get; set; }
    public string? LyDoKhauTru { get; set; }
    public bool IsDaHoanCoc { get; set; }

    // Handler Info
    public int? NguoiXuLyId { get; set; }
    public string? TenNguoiXuLy { get; set; }
    public DateTimeOffset? NgayXuLy { get; set; }
    public string? LyDo { get; set; } // Reason for return/rejection

    public int TotalCount { get; set; }
}
