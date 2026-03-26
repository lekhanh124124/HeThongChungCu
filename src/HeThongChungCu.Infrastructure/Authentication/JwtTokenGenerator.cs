using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HeThongChungCu.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _jwtOptions;
    private readonly IDateTimeProvider _dateTimeProvider;

    public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions, IDateTimeProvider dateTimeProvider)
    {
        _jwtOptions = jwtOptions.Value;
        _dateTimeProvider = dateTimeProvider;
    }

    public string GenerateToken(int accountId, string username, IEnumerable<string> roles, int? userId = null)
    {
        var claims = new List<Claim>
        {
            // "sub" (Subject) claim chuẩn JWT, dùng để lưu AccountId
            new Claim(JwtRegisteredClaimNames.Sub, accountId.ToString()), 
            new Claim(JwtRegisteredClaimNames.Name, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
 
        if (userId.HasValue)
        {
            claims.Add(new Claim("profile_id", userId.Value.ToString()));
        }

        // Add Roles claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = _dateTimeProvider.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes).UtcDateTime,
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(securityToken);
    }

    public int? GetUserIdFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        if (!tokenHandler.CanReadToken(token)) return null;

        var jwtToken = tokenHandler.ReadJwtToken(token);
        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "profile_id");

        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }

        return null;
    }
}
