namespace HeThongChungCu.Application.Common.Interfaces.Services;

public interface ITokenService
{
    int RefreshTokenExpiryMinutes { get; }
    string GenerateToken(int accountId, string username, IEnumerable<string> roles, int? userId = null);
    int? GetUserIdFromToken(string token);
    int? GetAccountIdFromToken(string token);
    string? GetJwtIdFromToken(string token);
}
