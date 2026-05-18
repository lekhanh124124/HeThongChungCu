using HeThongChungCu.Application.Common.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HeThongChungCu.Infrastructure.Embeddings;

public static class DependencyInjection
{
    public static IServiceCollection AddGeminiEmbeddings(this IServiceCollection services)
    {
        services.AddScoped<IEmbeddingService, GeminiEmbeddingService>();
        return services;
    }
}
