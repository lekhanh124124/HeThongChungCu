using Azure.Storage.Blobs;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Infrastructure.Common.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HeThongChungCu.Infrastructure.FileStorage;

public static class DependencyInjection
{
    public static IServiceCollection AddFileStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BlobStorageSettings>(configuration.GetSection(BlobStorageSettings.SectionName));
        services.Configure<FileCleanupSettings>(configuration.GetSection(FileCleanupSettings.SectionName));

        var settings = configuration.GetSection(BlobStorageSettings.SectionName).Get<BlobStorageSettings>();

        if (string.IsNullOrWhiteSpace(settings?.ConnectionString))
            throw new InvalidOperationException("Blob Storage connection string not configured.");

        services.AddSingleton(x => new BlobServiceClient(settings.ConnectionString));
        services.AddScoped<IFileStorageService, FileStorageService>();

        return services;
    }
}
