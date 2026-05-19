using System;
using System.IO;
using HeThongChungCu.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace HeThongChungCu.Infrastructure.Services;

public class MaintenanceService : IMaintenanceService
{
    private static bool _isMaintenanceActive = false;
    private readonly string _filePath;
    private readonly ILogger<MaintenanceService> _logger;

    public MaintenanceService(IWebHostEnvironment env, ILogger<MaintenanceService> logger)
    {
        _logger = logger;
        _filePath = Path.Combine(env.ContentRootPath, "maintenance.state");

        try
        {
            if (File.Exists(_filePath))
            {
                var content = File.ReadAllText(_filePath);
                _isMaintenanceActive = content.Trim().Equals("true", StringComparison.OrdinalIgnoreCase);
                _logger.LogInformation("Loaded maintenance state from file: {State}", _isMaintenanceActive);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load maintenance state from file, defaulting to false.");
            _isMaintenanceActive = false;
        }
    }

    public bool IsMaintenanceActive() => _isMaintenanceActive;

    public void SetMaintenanceMode(bool active)
    {
        _isMaintenanceActive = active;
        try
        {
            File.WriteAllText(_filePath, active ? "true" : "false");
            _logger.LogInformation("Saved maintenance state to file: {State}", active);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write maintenance state to file.");
        }
    }
}
