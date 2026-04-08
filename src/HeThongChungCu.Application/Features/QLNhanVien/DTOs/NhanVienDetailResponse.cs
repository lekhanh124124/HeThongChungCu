namespace HeThongChungCu.Application.Features.QLNhanVien.DTOs;

public class NhanVienDetailResponse : NhanVienResponse
{
    public int NguoiDungId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? CCCD { get; set; }
    public string? DiaChi { get; set; }
    public DateTimeOffset Dob { get; set; }
    public int GioiTinhId { get; set; }
    public string GioiTinhName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
    public string? GhiChu { get; set; }
    public List<TaiLieuNhanVienResponse> TaiLieuNguoiDungs { get; set; } = [];
}
