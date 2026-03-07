namespace HeThongChungCu.Application.Common.Interfaces.Services;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? Username { get; }
    string? UserEmail { get; }
    bool IsAuthenticated { get; }
    IEnumerable<string> Roles { get; }
}
