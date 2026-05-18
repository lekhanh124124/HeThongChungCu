using HeThongChungCu.Application.Common.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HeThongChungCu.Infrastructure.Gemini;

public static class DependencyInjection
{
    public static IServiceCollection AddGeminiLLM(this IServiceCollection services)
    {
        services.AddScoped<ILLMService, GeminiLLMService>();
        return services;
    }
}
