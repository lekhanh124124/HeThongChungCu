using FluentValidation;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.WebAPI.Common.Models;
using System.Net;
using System.Text.Json;

namespace HeThongChungCu.WebAPI.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, errors) = MapException(exception);

        context.Response.StatusCode = statusCode;

        var response = new ApiResponse<object>
        {
            IsOk = false,
            Errors = errors
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);

        await context.Response.WriteAsync(json);
    }

    private (int StatusCode, List<Error> Errors) MapException(Exception exception)
    {
        return exception switch
        {
            ValidationException validationEx => (
                (int)HttpStatusCode.BadRequest,
                validationEx.Errors.Select(e => new Error("Validation.Error", e.ErrorMessage)).ToList()
            ),

            NotFoundException notFoundEx => (
                (int)HttpStatusCode.NotFound,
                new List<Error> { new Error("Resource.NotFound", notFoundEx.Message) }
            ),

            DomainException domainEx => (
                (int)HttpStatusCode.BadRequest,
                new List<Error> { new Error("Domain.Error", domainEx.Message) }
            ),

            UnauthorizedAccessException => (
                (int)HttpStatusCode.Unauthorized,
                new List<Error> { new Error("Auth.Unauthorized", "Bạn không có quyền truy cập tài nguyên này.") }
            ),

            _ => (
                (int)HttpStatusCode.InternalServerError,
                new List<Error> { new Error("System.Exception", "Đã xảy ra lỗi hệ thống.") }
            )
        };
    }
}
