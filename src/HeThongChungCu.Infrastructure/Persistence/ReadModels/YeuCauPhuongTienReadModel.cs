namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record YeuCauPhuongTienReadModel
{
    public int TotalCount { get; init; }
    public int Id { get; init; }
    public int CanHoId { get; init; }
    public int? YeuCauPhuongTienId { get; init; } // Target vehicle ID for Update/Delete
    public int LoaiHanhDongYeuCauId { get; init; }
    public int TrangThaiId { get; init; }
    public string? LyDo { get; init; }
    public string? NoiDung { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? NgayXuLy { get; init; }
    public int? NguoiXuLyId { get; init; }
    public int CreatedBy { get; init; }

    // Proximity data
    public string YeuCauTenPhuongTien { get; init; } = string.Empty;
    public int YeuCauLoaiPhuongTienId { get; init; }
    public string YeuCauBienSo { get; init; } = string.Empty;
    public string YeuCauMauXe { get; init; } = string.Empty;

    // Join fields
    public string TenCanHo { get; init; } = null!;
    public string TenTang { get; init; } = null!;
    public string TenToaNha { get; init; } = null!;
    public string TenNguoiGui { get; init; } = null!;
    public string? TenNguoiXuLy { get; init; }
}
