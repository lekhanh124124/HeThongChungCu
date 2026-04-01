using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Infrastructure.Common.Settings;
using HeThongChungCu.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HeThongChungCu.Infrastructure.Authentication;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthLayer(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Password Hashing
        services.AddTransient<IHasherService, HasherService>();

        // 2. Token Service
        services.AddTransient<ITokenService, JwtTokenService>();

        // 3. System DateTime
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        // 4. Bind JWT Settings from appsettings.json
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.SectionName, jwtSettings);
        services.AddSingleton(Options.Create(jwtSettings));

        // 5. Configure Authentication
        services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                };
            });

        return services;
    }
}
