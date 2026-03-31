namespace HeThongChungCu.Application.Features.QLCuTru.DTOs;

public record DSYeuCauCuTruResponse
{
    // Thông tin người gửi
    public int Id { get; init; }
    public int CreatedBy { get; init; }
    public string TenNguoiGui { get; init; } = null!;
    public DateTimeOffset CreatedAt { get; init; }
    public int CanHoId { get; init; }
    public string TenCanHo { get; init; } = null!;
    public string TenTang { get; init; } = null!;
    public string TenToaNha { get; init; } = null!;

    // Thông tin người xử lý
    public int? NguoiXuLyId { get; init; }
    public string? TenNguoiXuLy { get; init; }
    public DateTimeOffset? NgayXuLy { get; init; }

    // Chi tiết yêu cầu
    public int LoaiYeuCauId { get; init; }
    public string TenLoaiYeuCau { get; init; } = null!;
    public int TrangThaiId { get; init; }
    public string TenTrangThai { get; init; } = null!;
    public string? LyDo { get; init; }
    public string? NoiDung { get; init; }
}
