namespace HeThongChungCu.Application.Features.QLCuTru.DTOs;

public class UserInfoResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime Dob { get; set; }
    public int GioiTinhId { get; set; }
    public string GioiTinhName { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string DiaChi { get; set; } = string.Empty;
    public string? IdCard { get; set; }

    public List<TaiLieuResponse> TaiLieuCuTrus { get; set; } = [];
}
