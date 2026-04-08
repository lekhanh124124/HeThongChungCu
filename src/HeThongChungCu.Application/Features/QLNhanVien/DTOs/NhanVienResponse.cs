namespace HeThongChungCu.Application.Features.QLNhanVien.DTOs;

public class NhanVienResponse
{
    public int Id { get; set; }
    public string AnhDaiDienUrl { get; set; } = string.Empty;
    public string MaNhanVien { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SoDienThoai { get; set; } = string.Empty;
    public int LoaiNhanVienId { get; set; }
    public string LoaiNhanVienTen { get; set; } = string.Empty;
    public int TrangThaiNhanVienId { get; set; }
    public string TrangThaiNhanVienTen { get; set; } = string.Empty;
    public DateTimeOffset NgayVaoLam { get; set; }
    public DateTimeOffset? NgayNghiLam { get; set; }
}
