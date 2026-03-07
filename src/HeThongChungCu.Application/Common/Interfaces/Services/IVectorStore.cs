namespace HeThongChungCu.Application.Common.Interfaces.Services;

public interface IVectorStore
{
    Task UpsertVectorAsync(string collectionName, int id, float[] vector, Dictionary<string, object>? payload = null, CancellationToken cancellationToken = default);
    Task DeleteVectorAsync(string collectionName, int id, CancellationToken cancellationToken = default);
    Task<List<int>> SearchSimilarAsync(string collectionName, float[] queryVector, int limit = 5, CancellationToken cancellationToken = default);
    Task CreateCollectionIfNotExistsAsync(string collectionName, ulong vectorSize, CancellationToken cancellationToken = default);
}
