using HealthChecks.UI.Client;
using HeThongChungCu.Application;
using HeThongChungCu.Application.Common.Options;
using HeThongChungCu.Infrastructure;
using HeThongChungCu.Infrastructure.Persistence;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;

namespace HeThongChungCu.WebAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ================= CORE SERVICES =================
            builder.Services.AddApplicationCore();
            builder.Services.AddInfrastructureLayer(builder.Configuration);
            builder.Services.AddWebAPIServices(builder.Configuration);

            // ================= LOGGING =================
            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentName()
                    .WriteTo.Console()
                    .WriteTo.File(
                        path: Path.Combine(context.HostingEnvironment.ContentRootPath, "Logs", "app_log-.txt"),
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        outputTemplate:
                        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                    );

                if (!context.HostingEnvironment.IsDevelopment())
                {
                    configuration.WriteTo.ApplicationInsights(
                        services.GetRequiredService<TelemetryConfiguration>(),
                        TelemetryConverter.Traces);
                }
            });

            // ================== BUILD APP ==================
            var app = builder.Build();

            // ================== INITIALISE AND SEED DATABASE ==================
            using (var scope = app.Services.CreateScope())
            {
                var logger = scope.ServiceProvider
                    .GetRequiredService<ILogger<Program>>();

                try
                {
                    var initialiser = scope.ServiceProvider
                        .GetRequiredService<ApplicationDbContextInitialiser>();

                    await initialiser.InitialiseAsync();
                    initialiser.Seed();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Database initialisation failed.");
                }
            }

            // ================== CONFIGURE THE HTTP REQUEST PIPELINE ==================
            app.UseMiddleware<Middlewares.GlobalExceptionMiddleware>();

            app.UseSerilogRequestLogging();

            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            app.UseSwagger();
            app.UseSwaggerUI();


            app.UseRouting();

            app.UseCors("AllowAll");

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // ================== MAP ENDPOINT FOR HEALTH CHECKS ==================
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

            app.MapGet("/debug/apis", (ISwaggerProvider swaggerProvider) =>
            {
                var doc = swaggerProvider.GetSwagger("v1");

                var apis = doc.Paths
                    .SelectMany(path => path.Value.Operations.Select(op => new
                    {
                        Route = path.Key,
                        Method = op.Key.ToString().ToUpper(),
                        op.Value.Summary,
                        op.Value.Description
                    }));

                return Results.Ok(apis);
            }).ExcludeFromDescription();

            app.Run();
        }
    }
}
