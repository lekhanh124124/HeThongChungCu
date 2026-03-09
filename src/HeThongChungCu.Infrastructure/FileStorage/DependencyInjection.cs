using Azure.Storage.Blobs;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Options;

namespace HeThongChungCu.Infrastructure.FileStorage;

public static class DependencyInjection
{
    public static IServiceCollection AddFileStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(FileStorageOptions.SectionName).Get<FileStorageOptions>();
        services.Configure<FileStorageOptions>(configuration.GetSection(FileStorageOptions.SectionName));

        if (string.IsNullOrWhiteSpace(options?.ConnectionString))
            throw new InvalidOperationException("Blob Storage connection string not configured.");

        services.AddSingleton(x => new BlobServiceClient(options.ConnectionString));
        services.AddScoped<IFileStorageService, FileStorageService>();

        return services;
    }
}
