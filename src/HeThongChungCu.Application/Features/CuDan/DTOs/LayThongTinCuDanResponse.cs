using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.CuDan.DTOs;

public class LayThongTinCuDanResponse
{
    // Thông tin người dùng
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int GioiTinhId { get; set; }
    public string GioiTinhName { get; set; } = string.Empty;
    public DateTimeOffset Dob { get; set; }
    public string? IdCard { get; set; }
    public string? PhoneNumber { get; set; }
    public string? DiaChi { get; set; }
    public string AnhDaiDienUrl { get; set; } = string.Empty;

    // Thông tin quan hệ cư trú
    public int QuanHeCuTruId { get; set; }
    public int LoaiQuanHeCuTruId { get; set; }
    public string LoaiQuanHeTen { get; set; } = string.Empty;
    public DateTimeOffset NgayBatDau { get; set; }
    public DateTimeOffset? NgayKetThuc { get; set; }
    public int TrangThaiCuTruId { get; set; }
    public string TrangThaiCuTruTen { get; set; } = string.Empty;
    public List<TaiLieuResponse> TaiLieuCuTrus { get; set; } = [];
}
