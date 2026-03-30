namespace HeThongChungCu.Application.Features.QLCuTru.DTOs;

public class UserInfoResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public int GioiTinhId { get; set; }
    public string GioiTinhName { get; set; } = string.Empty;
    public DateTime Dob { get; set; }
    public string? IdCard { get; set; }
    public string? PhoneNumber { get; set; }
    public string? DiaChi { get; set; }
    public int LoaiQuanHeCuTruId { get; set; }
    public string TenLoaiQuanHeCuTru { get; set; } = string.Empty;

    public List<TaiLieuResponse> TaiLieuCuTrus { get; set; } = [];
}
