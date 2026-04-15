using Asp.Versioning;
using HeThongChungCu.Infrastructure.Common.Settings;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.WebAPI.Common.Models;
using HeThongChungCu.WebAPI.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace HeThongChungCu.WebAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;

            // Keep routes unchanged; clients can specify version via query string or header.
            options.ApiVersionReader = new QueryStringApiVersionReader("api-version");
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
        });

        services.Configure<ApplicationInsightsSettings>(
            configuration.GetSection(ApplicationInsightsSettings.SectionName));

        services.AddApplicationInsightsTelemetry();

        services.AddHttpContextAccessor();
        services.AddAuthorization();

        services.Configure<JwtBearerOptions>(
            JwtBearerDefaults.AuthenticationScheme,
            options =>
        {
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notifications"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                },

                OnChallenge = async context =>
                {
                    // Skip challenge if the endpoint allows anonymous access or doesn't require authorization
                    var endpoint = context.HttpContext.GetEndpoint();
                    if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null ||
                        endpoint?.Metadata?.GetMetadata<IAuthorizeData>() == null)
                    {
                        return;
                    }

                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                    var error = context.AuthenticateFailure is SecurityTokenExpiredException
                        ? AuthErrors.TokenExpired
                        : AuthErrors.Unauthorized;

                    await context.Response.WriteAsJsonAsync(new ApiResponse<object>
                    {
                        IsOk = false,
                        Errors = [error]
                    });
                },

                OnForbidden = async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;

                    await context.Response.WriteAsJsonAsync(new ApiResponse<object>
                    {
                        IsOk = false,
                        Errors = [AuthErrors.Forbidden]
                    });
                }
            };
        });

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.SetIsOriginAllowed(_ => true)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        services.AddSwaggerGen();
        services.ConfigureOptions<ConfigureSwaggerOptions>();

        return services;
    }
}
