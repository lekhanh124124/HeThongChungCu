namespace HeThongChungCu.Application.Features.NhanVien.DTOs;

public class NhanVienResponse
{
    public int Id { get; set; }
    public int NguoiDungId { get; set; }
    public string HoTen { get; set; } = null!;
    public string SoDienThoai { get; set; } = null!;
    public int LoaiNhanVienId { get; set; }
    public string TenLoaiNhanVien { get; set; } = null!;
    public int TrangThaiNhanVienId { get; set; }
    public string TenTrangThaiNhanVien { get; set; } = null!;
    public string MaNhanVien { get; set; } = null!;
    public DateTime NgayVaoLam { get; set; }
    public DateTime? NgayNghiLam { get; set; }
    public string? GhiChu { get; set; }
}
