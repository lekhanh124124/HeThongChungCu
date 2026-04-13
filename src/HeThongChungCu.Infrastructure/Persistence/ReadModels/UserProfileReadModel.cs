namespace HeThongChungCu.Infrastructure.Persistence.ReadModels;

internal record UserProfileReadModel
{
    public int Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public DateTimeOffset? Dob { get; init; }
    public string? DiaChi { get; init; }
    public int GioiTinhId { get; init; }
    public string? AnhDaiDienUrl { get; init; }
}
