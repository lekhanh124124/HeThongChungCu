namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal class DichVuDetailReadModel
{
    // DichVu fields
    public int Id { get; set; }
    public string MaDichVu { get; set; } = string.Empty;
    public string TenDichVu { get; set; } = string.Empty;
    public int LoaiDichVuId { get; set; }
    public string DonViTinh { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public bool IsBatBuoc { get; set; }
    public int? SoLuongToiDa { get; set; }
    public int TrangThaiId { get; set; }
    public string? IconUrl { get; set; }

    // KhungGioDichVu fields (Main slot)
    public int? KhungGioId { get; set; }
    public TimeSpan? GioBatDau { get; set; }
    public TimeSpan? GioKetThuc { get; set; }
    public string? TenKhungGio { get; set; }
    public int? NgayTrongTuan { get; set; }

    // BangGia fields
    public int? BangGiaId { get; set; }
    public string? TenBangGia { get; set; }
    public DateTimeOffset? NgayApDung { get; set; }
    public DateTimeOffset? NgayKetThuc { get; set; }
    public int? LoaiDinhGiaId { get; set; }
    public decimal? DonGia { get; set; }
    public bool? IsActive { get; set; }

    // ChiTietGiaLuyTien fields
    public int? GiaLuyTienId { get; set; }
    public decimal? TuMuc { get; set; }
    public decimal? DenMuc { get; set; }
    public decimal? DonGiaLuyTien { get; set; }

    // ChiTietGiaKhungGio fields
    public int? GiaKhungGioId { get; set; }
    public int? KhungGioId_Detail { get; set; }
    public decimal? DonGiaKhungGio { get; set; }
    public string? TenKhungGio_Detail { get; set; }

    // ChiTietGiaLoaiCanHo fields
    public int? GiaLoaiCanHoId { get; set; }
    public int? LoaiCanHoId { get; set; }
    public decimal? DonGiaLoaiCanHo { get; set; }
}
