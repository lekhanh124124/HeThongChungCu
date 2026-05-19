using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.WebAPI.Common.Models;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.WebAPI.Middlewares;

public class MaintenanceModeMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly List<string> BypassRoutes = new()
    {
        "/api/backup/restore", // Cho phép gọi API restore
        "/api/auth/login",     // Cho phép đăng nhập Admin
        "/health"              // Cho phép Health Check hệ thống
    };

    public MaintenanceModeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IMaintenanceService maintenanceService)
    {
        if (maintenanceService.IsMaintenanceActive())
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";
            
            bool isBypass = BypassRoutes.Any(r => path.StartsWith(r.ToLower()));
            bool isAdmin = context.User.Identity?.IsAuthenticated == true && context.User.IsInRole("Admin");

            if (!isBypass && !isAdmin)
            {
                context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                context.Response.ContentType = "application/json";

                var response = new ApiResponse<object>
                {
                    IsOk = false,
                    Errors = new List<Error> 
                    { 
                        new Error("System.Maintenance", "Hệ thống đang bảo trì để khôi phục dữ liệu. Vui lòng quay lại sau ít phút.") 
                    }
                };

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
                return;
            }
        }

        await _next(context);
    }
}
