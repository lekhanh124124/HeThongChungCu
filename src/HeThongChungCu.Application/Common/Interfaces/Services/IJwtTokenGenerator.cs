namespace HeThongChungCu.Application.Common.Interfaces.Services;

public interface IJwtTokenGenerator
{
    string GenerateToken(int userId, string username, IEnumerable<string> roles);
}
