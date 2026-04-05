namespace HeThongChungCu.Application.Features.Profile.DTOs;

public class UserProfileDetailResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string DiaChi { get; set; } = string.Empty;
    public DateTimeOffset Dob { get; set; }
    public int GioiTinhId { get; set; }
    public string GioiTinhName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
    public string AnhDaiDienUrl { get; set; } = string.Empty;
}
