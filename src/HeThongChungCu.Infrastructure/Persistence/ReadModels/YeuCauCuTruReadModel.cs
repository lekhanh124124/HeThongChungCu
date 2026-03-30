namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

public record YeuCauCuTruReadModel
{
    public int TotalCount { get; init; }
    public int Id { get; init; }
    public int CanHoId { get; init; }
    public int LoaiYeuCauId { get; init; }
    public int TrangThaiId { get; init; }
    public string? LyDo { get; init; }
    public string? NoiDung { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTime? NgayXuLy { get; init; }
    public int? NguoiXuLyId { get; init; }

    // Flattened fields
    public string? YeuCauTen { get; init; }
    public string? YeuCauHo { get; init; }
    public DateTime? YeuCauNgaySinh { get; init; }
    public int? YeuCauGioiTinhId { get; init; }
    public string? YeuCauSoDienThoai { get; init; }
    public string? YeuCauCCCD { get; init; }
    public string? YeuCauDiaChi { get; init; }
    public int? YeuCauLoaiQuanHeId { get; init; }
    public int? QuanHeCuTruId { get; init; }
}
