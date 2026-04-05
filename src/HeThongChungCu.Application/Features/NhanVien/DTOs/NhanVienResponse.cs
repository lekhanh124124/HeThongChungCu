using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.NhanVien.DTOs;

public class NhanVienResponse
{
    public int Id { get; set; }
    public int NguoiDungId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string HoTen { get; set; } = null!;
    public string Email { get; set; } = string.Empty;
    public string SoDienThoai { get; set; } = null!;
    public string? CCCD { get; set; }
    public string? DiaChi { get; set; }
    public DateTimeOffset Dob { get; set; }
    public int GioiTinhId { get; set; }
    public string GioiTinhName { get; set; } = string.Empty;
    public string? AnhDaiDienUrl { get; set; }
    public List<string> Roles { get; set; } = [];
    public int LoaiNhanVienId { get; set; }
    public string TenLoaiNhanVien { get; set; } = null!;
    public int TrangThaiNhanVienId { get; set; }
    public string TenTrangThaiNhanVien { get; set; } = null!;
    public string MaNhanVien { get; set; } = null!;
    public DateTimeOffset NgayVaoLam { get; set; }
    public DateTimeOffset? NgayNghiLam { get; set; }
    public string? GhiChu { get; set; }
    public List<TaiLieuResponse> TaiLieuNguoiDungs { get; set; } = [];
}
