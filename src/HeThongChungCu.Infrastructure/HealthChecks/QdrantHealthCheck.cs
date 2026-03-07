using Microsoft.Extensions.Diagnostics.HealthChecks;
using Qdrant.Client;

namespace HeThongChungCu.Infrastructure.HealthChecks;

public class QdrantHealthCheck : IHealthCheck
{
    private readonly QdrantClient _qdrantClient;

    public QdrantHealthCheck(QdrantClient qdrantClient)
    {
        _qdrantClient = qdrantClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Kiểm tra kết nối Qdrant
            var result = await _qdrantClient.ListCollectionsAsync(cancellationToken);
            return HealthCheckResult.Healthy("Qdrant is running normally.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message);
        }
    }
}
