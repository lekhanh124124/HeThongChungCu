using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Options;
using HeThongChungCu.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HeThongChungCu.Infrastructure.Authentication;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthLayer(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Password Hashing
        services.AddTransient<IPasswordHasher, PasswordHasher>();

        // 2. JWT Generator
        services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();

        // 3. System DateTime
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        // 4. Bind JWT Settings from appsettings.json
        var jwtSettings = new JwtOptions();
        configuration.Bind(JwtOptions.SectionName, jwtSettings);
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
