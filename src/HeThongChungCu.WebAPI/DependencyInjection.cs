using HeThongChungCu.Infrastructure.Common.Settings;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.WebAPI.Common.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;

namespace HeThongChungCu.WebAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();

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

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hệ thống Quản lý Chung cư API", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
            c.DocInclusionPredicate((name, api) => true);

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }
}
