using HeThongChungCu.Infrastructure.Authentication;
using HeThongChungCu.Infrastructure.Email;
using HeThongChungCu.Infrastructure.FileStorage;
using HeThongChungCu.Infrastructure.HealthChecks;
using HeThongChungCu.Infrastructure.Persistence;
using HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;
using HeThongChungCu.Infrastructure.Qdrant;
using HeThongChungCu.Infrastructure.Services;
using HeThongChungCu.Infrastructure.Gemini;
using HeThongChungCu.Infrastructure.OpenAI;
using HeThongChungCu.Infrastructure.Embeddings;
using HeThongChungCu.Infrastructure.Chunking;
using HeThongChungCu.Infrastructure.Notifications;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HeThongChungCu.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddServices();
        services.AddAuthLayer(configuration);
        services.AddHealthChecks(configuration);
        services.AddEmail(configuration);
        services.AddQdrantVectorStore(configuration);
        
        // Dynamically select LLM provider from configuration (default is Gemini)
        var aiProvider = configuration["AI:Provider"] ?? "Gemini";
        if (aiProvider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase))
        {
            services.AddOpenAILLM();
            services.AddOpenAIEmbeddings();
        }
        else
        {
            services.AddGeminiLLM();
            services.AddGeminiEmbeddings();
        }
        services.AddChunkingServices();
        services.AddFileStorage(configuration);
        services.AddNotification();
        services.AddMemoryCache();
        return services;
    }
}
