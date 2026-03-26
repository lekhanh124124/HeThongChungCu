namespace HeThongChungCu.Application.Common.Interfaces.Services;

public interface IJwtTokenGenerator
{
    string GenerateToken(int accountId, string username, IEnumerable<string> roles, int? userId = null);
    int? GetUserIdFromToken(string token);
}
