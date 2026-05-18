using HeThongChungCu.Application.Common.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HeThongChungCu.Infrastructure.Chunking;

public static class DependencyInjection
{
    public static IServiceCollection AddChunkingServices(this IServiceCollection services)
    {
        services.AddSingleton<ITextChunker, MarkdigChunker>();
        return services;
    }
}
