using HeThongChungCu.Application.Features.QLCuTru.DTOs;

namespace HeThongChungCu.Application.Features.CuDan.DTOs;

public class LayThongTinCuDanResponse
{
    // Thông tin người dùng
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime Dob { get; set; }
    public int GioiTinhId { get; set; }
    public string GioiTinhName { get; set; } = string.Empty;
    public string AnhDaiDienUrl { get; set; } = string.Empty;
    public List<TaiLieuResponse> TaiLieuCuTrus { get; set; } = [];

    // Thông tin quan hệ cư trú
    public int QuanHeCuTruId { get; set; }
    public int LoaiQuanHeCuTruId { get; set; }
    public string LoaiQuanHeTen { get; set; } = string.Empty;
    public DateTime NgayBatDau { get; set; }
    public string? IdCard { get; set; }
}
