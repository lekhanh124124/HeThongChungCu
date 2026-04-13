namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record YeuCauCuTruReadModel
{
    public int TotalCount { get; init; }
    public int Id { get; init; }
    public int CanHoId { get; init; }
    public int LoaiYeuCauId { get; init; }
    public int TrangThaiId { get; init; }
    public string? LyDo { get; init; }
    public string? NoiDung { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? NgayXuLy { get; init; }
    public int? NguoiXuLyId { get; init; }
    public int CreatedBy { get; init; }

    // Flattened fields
    public string? YeuCauTen { get; init; }
    public string? YeuCauHo { get; init; }
    public DateTimeOffset? YeuCauNgaySinh { get; init; }
    public int? YeuCauGioiTinhId { get; init; }
    public string? YeuCauSoDienThoai { get; init; }
    public string? YeuCauCCCD { get; init; }
    public string? YeuCauDiaChi { get; init; }
    public int? YeuCauLoaiQuanHeId { get; init; }
    public int? YeuCauQuanHeCuTruId { get; init; }

    // Join fields
    public string TenCanHo { get; init; } = null!;
    public string TenTang { get; init; } = null!;
    public string TenToaNha { get; init; } = null!;
    public string TenNguoiGui { get; init; } = null!;
    public string? TenNguoiXuLy { get; init; }
    
    // Filter fields from join
    public int ToaNhaId { get; init; }
    public int TangId { get; init; }
}
