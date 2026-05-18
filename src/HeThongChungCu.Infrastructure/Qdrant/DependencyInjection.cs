using HeThongChungCu.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qdrant.Client;

namespace HeThongChungCu.Infrastructure.Qdrant;

public static class DependencyInjection
{
    public static IServiceCollection AddQdrantVectorStore(this IServiceCollection services, IConfiguration configuration)
    {
        var host = configuration["Qdrant:Host"] ?? "localhost";
        var port = int.Parse(configuration["Qdrant:Port"] ?? "6334"); // GRPC port
        var apiKey = configuration["Qdrant:ApiKey"];
        var useHttps = bool.Parse(configuration["Qdrant:UseHttps"] ?? "false");

        // Đăng ký thư viện Qdrant.Client gốc
        services.AddSingleton(new QdrantClient(host, port, https: useHttps, apiKey: apiKey));

        // Đăng ký Wrapper của ta tuân thủ IVectorStore interface
        services.AddTransient<IVectorStore, QdrantVectorStore>();

        return services;
    }
}
