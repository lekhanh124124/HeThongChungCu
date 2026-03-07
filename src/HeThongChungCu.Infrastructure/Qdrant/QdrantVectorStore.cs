using HeThongChungCu.Application.Common.Interfaces.Services;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace HeThongChungCu.Infrastructure.Qdrant;

/// <summary>
/// Wrapper bao đóng Qdrant.Client. Quá trình trao đổi qua lại giữa Application và thư viện Vector DB
/// sẽ đi qua Wrapper này thông qua IVectorStore interface.
/// Application KHÔNG CẦN BIẾT Qdrant là cái gì, nó chỉ nhận trả về primitive types.
/// </summary>
public class QdrantVectorStore : IVectorStore
{
    private readonly QdrantClient _client;

    public QdrantVectorStore(QdrantClient client)
    {
        _client = client;
    }

    public async Task CreateCollectionIfNotExistsAsync(string collectionName, ulong vectorSize, CancellationToken cancellationToken = default)
    {
        var collectionsResult = await _client.ListCollectionsAsync(cancellationToken);

        if (!collectionsResult.Contains(collectionName))
        {
            await _client.CreateCollectionAsync(
                collectionName: collectionName,
                // Cosine là độ đo phổ biến nhất cho So khớp văn bản / RAG (Retrieval-Augmented Generation)
                vectorsConfig: new VectorParams { Size = vectorSize, Distance = Distance.Cosine },
                cancellationToken: cancellationToken);
        }
    }

    public async Task UpsertVectorAsync(string collectionName, int id, float[] vector, Dictionary<string, object>? payload = null, CancellationToken cancellationToken = default)
    {
        var pointStruct = new PointStruct
        {
            Id = (ulong)id, // Qdrant dùng ulong cho Point ID
            Vectors = vector
        };

        if (payload != null && payload.Any())
        {
            foreach (var kvp in payload)
            {
                // Simple mapping for primitive types (demo purpose)
                if (kvp.Value is string s) pointStruct.Payload.Add(kvp.Key, s);
                else if (kvp.Value is int i) pointStruct.Payload.Add(kvp.Key, i);
                else if (kvp.Value is bool b) pointStruct.Payload.Add(kvp.Key, b);
                // In production, you'd want a more robust mapping here or use Payload.FromDictionary() if available
            }
        }

        await _client.UpsertAsync(collectionName, new[] { pointStruct }, cancellationToken: cancellationToken);
    }

    public async Task DeleteVectorAsync(string collectionName, int id, CancellationToken cancellationToken = default)
    {
        await _client.DeleteAsync(collectionName, new ulong[] { (ulong)id }, cancellationToken: cancellationToken);
    }

    public async Task<List<int>> SearchSimilarAsync(string collectionName, float[] queryVector, int limit = 5, CancellationToken cancellationToken = default)
    {
        // Thực hiện Approximate Nearest Neighbor (ANN) search bằng Cosine Similarity đã setup
        var searchResult = await _client.SearchAsync(
            collectionName: collectionName,
            vector: queryVector,
            limit: (ulong)limit,
            cancellationToken: cancellationToken);

        // Map kết quả ulong ID của Qdrant về int ID để trả cho Application layer
        return searchResult.Select(point => (int)point.Id.Num).ToList();
    }
}
