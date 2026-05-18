using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Common.Interfaces.Services;

public interface IVectorStore
{
    Task CreateCollectionIfNotExistsAsync(string collectionName, ulong vectorSize, CancellationToken cancellationToken = default);
    Task DeleteCollectionAsync(string collectionName, CancellationToken cancellationToken = default);
    Task CreatePayloadIndexAsync(string collectionName, string fieldName, string schemaType, CancellationToken cancellationToken = default);
    Task UpsertVectorAsync(string collectionName, string id, float[] vector, Dictionary<string, object>? payload = null, CancellationToken cancellationToken = default);
    Task UpsertVectorsBatchAsync(string collectionName, List<VectorRecord> records, CancellationToken cancellationToken = default);
    Task DeleteVectorAsync(string collectionName, string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Xóa tất cả vector trong collection có payload field "source" bằng giá trị chỉ định.
    /// Dùng để dọn sạch chunk mồ côi (orphaned chunks) trước khi đồng bộ lại file đã thay đổi.
    /// </summary>
    Task DeleteBySourceAsync(string collectionName, string sourceValue, CancellationToken cancellationToken = default);

    Task<List<VectorSearchResult>> SearchSimilarAsync(
        string collectionName,
        float[] queryVector,
        int limit = 5,
        Dictionary<string, object>? filterMetadata = null,
        CancellationToken cancellationToken = default);
}

