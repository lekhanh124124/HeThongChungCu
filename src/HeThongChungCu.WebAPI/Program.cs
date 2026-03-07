using HealthChecks.UI.Client;
using HeThongChungCu.Application;
using HeThongChungCu.Infrastructure;
using HeThongChungCu.WebAPI.Common.Logging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace HeThongChungCu.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Core Services Definition
            builder.Services.AddApplicationCore();
            builder.Services.AddInfrastructureLayer(builder.Configuration);
            builder.Services.AddWebAPIServices();

            // 1.1 Logging setup
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            var logPath = Path.Combine(builder.Environment.ContentRootPath, "Logs", "app_log.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
            builder.Logging.AddFileLogger(logPath);

            // ================== BUILD APP ==================
            var app = builder.Build();

            // 2. Configure the HTTP request pipeline.
            app.UseMiddleware<Middlewares.GlobalExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();

            app.UseCors("AllowAll");

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Map Endpoint cho Health Checks
            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.Run();
        }
    }
}
