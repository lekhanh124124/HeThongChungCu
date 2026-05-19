using HeThongChungCu.Application.Common.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HeThongChungCu.Infrastructure.OpenAI;

public static class DependencyInjection
{
    public static IServiceCollection AddOpenAILLM(this IServiceCollection services)
    {
        services.AddScoped<ILLMService, OpenAILLMService>();
        return services;
    }

    public static IServiceCollection AddOpenAIEmbeddings(this IServiceCollection services)
    {
        services.AddScoped<IEmbeddingService, OpenAIEmbeddingService>();
        return services;
    }
}
