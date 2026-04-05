namespace HeThongChungCu.Application.Features.QLCuTru.DTOs;

public record YeuCauCuTruResponse
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
    public int? TargetQuanHeCuTruId { get; init; }

    public string? YeuCauTen { get; init; }
    public string? YeuCauHo { get; init; }
    public DateTimeOffset? YeuCauNgaySinh { get; init; }
    public int? YeuCauGioiTinhId { get; init; }
    public string? YeuCauGioiTinhTen { get; init; }
    public string? YeuCauSoDienThoai { get; init; }
    public string? YeuCauCCCD { get; init; }
    public string? YeuCauDiaChi { get; init; }
    public int? YeuCauLoaiQuanHeId { get; init; }
    public string? YeuCauLoaiQuanHeTen { get; init; }

    public string? NoiDung { get; init; }
    public string? LyDo { get; init; }
    public int TrangThaiId { get; init; }
    public string TenTrangThai { get; init; } = null!;

    public List<TaiLieuResponse> Documents { get; init; } = [];
}
