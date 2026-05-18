using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Models;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Infrastructure.Qdrant;

/// <summary>
/// Wrapper bao đóng Qdrant.Client. Quá trình trao đổi qua lại giữa Application và thư viện Vector DB
/// sẽ đi qua Wrapper này thông qua IVectorStore interface.
/// Application KHÔNG CẦN BIẾT Qdrant là cái gì, nó chỉ nhận trả về primitive types / DTOs chung.
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
        if (string.IsNullOrWhiteSpace(collectionName))
            throw new ArgumentException("Collection name must not be null or empty.", nameof(collectionName));

        var collectionsResult = await _client.ListCollectionsAsync(cancellationToken);

        if (!collectionsResult.Contains(collectionName))
        {
            await _client.CreateCollectionAsync(
                collectionName: collectionName,
                // Cosine là độ đo tương đồng tối ưu cho So khớp văn bản / RAG (Retrieval-Augmented Generation)
                vectorsConfig: new VectorParams { Size = vectorSize, Distance = Distance.Cosine },
                cancellationToken: cancellationToken);
        }
    }

    public async Task DeleteCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
            throw new ArgumentException("Collection name must not be null or empty.", nameof(collectionName));

        var collectionsResult = await _client.ListCollectionsAsync(cancellationToken);

        if (collectionsResult.Contains(collectionName))
        {
            await _client.DeleteCollectionAsync(collectionName, cancellationToken: cancellationToken);
        }
    }

    public async Task CreatePayloadIndexAsync(string collectionName, string fieldName, string schemaType, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
            throw new ArgumentException("Collection name must not be null or empty.", nameof(collectionName));
        if (string.IsNullOrWhiteSpace(fieldName))
            throw new ArgumentException("Field name must not be null or empty.", nameof(fieldName));
        if (string.IsNullOrWhiteSpace(schemaType))
            throw new ArgumentException("Schema type must not be null or empty.", nameof(schemaType));

        global::Qdrant.Client.Grpc.PayloadSchemaType qdrantSchemaType = schemaType.ToLower() switch
        {
            "keyword" => global::Qdrant.Client.Grpc.PayloadSchemaType.Keyword,
            "integer" => global::Qdrant.Client.Grpc.PayloadSchemaType.Integer,
            "float" => global::Qdrant.Client.Grpc.PayloadSchemaType.Float,
            "bool" => global::Qdrant.Client.Grpc.PayloadSchemaType.Bool,
            "text" => global::Qdrant.Client.Grpc.PayloadSchemaType.Text,
            _ => throw new ArgumentException($"Unsupported schema type: {schemaType}", nameof(schemaType))
        };

        // Gọi tạo payload index trực tiếp. Ném ngoại lệ rõ ràng nếu cấu hình thất bại (Không nuốt lỗi)
        await _client.CreatePayloadIndexAsync(
            collectionName: collectionName,
            fieldName: fieldName,
            schemaType: qdrantSchemaType,
            cancellationToken: cancellationToken);
    }

    public async Task UpsertVectorAsync(string collectionName, string id, float[] vector, Dictionary<string, object>? payload = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
            throw new ArgumentException("Collection name must not be null or empty.", nameof(collectionName));
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Vector ID must not be null or empty.", nameof(id));
        if (vector == null || vector.Length == 0)
            throw new ArgumentException("Vector data must not be null or empty.", nameof(vector));

        var pointStruct = new PointStruct
        {
            Id = MapToPointId(id),
            Vectors = vector
        };

        MapPayload(pointStruct.Payload, payload);

        await _client.UpsertAsync(collectionName, new[] { pointStruct }, cancellationToken: cancellationToken);
    }

    public async Task UpsertVectorsBatchAsync(string collectionName, List<VectorRecord> records, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
            throw new ArgumentException("Collection name must not be null or empty.", nameof(collectionName));
        if (records == null || !records.Any()) return;

        if (records.Any(r => r.Vector == null || r.Vector.Length == 0))
            throw new ArgumentException("One or more records contain null or empty vectors.");
        if (records.Any(r => string.IsNullOrWhiteSpace(r.Id)))
            throw new ArgumentException("One or more records contain null or empty ID.");

        var points = records.Select(r =>
        {
            var point = new PointStruct
            {
                Id = MapToPointId(r.Id),
                Vectors = r.Vector
            };
            MapPayload(point.Payload, r.Payload);
            return point;
        }).ToList();

        // Nạp toàn bộ danh sách điểm trong 1 gRPC request duy nhất
        await _client.UpsertAsync(collectionName, points, cancellationToken: cancellationToken);
    }

    public async Task DeleteVectorAsync(string collectionName, string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
            throw new ArgumentException("Collection name must not be null or empty.", nameof(collectionName));
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Vector ID must not be null or empty.", nameof(id));

        await _client.DeleteAsync(collectionName, new[] { MapToPointId(id) }, cancellationToken: cancellationToken);
    }

    public async Task DeleteBySourceAsync(string collectionName, string sourceValue, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
            throw new ArgumentException("Collection name must not be null or empty.", nameof(collectionName));
        if (string.IsNullOrWhiteSpace(sourceValue))
            throw new ArgumentException("Source value must not be null or empty.", nameof(sourceValue));

        // Xóa tất cả điểm có payload field "source" khớp chính xác với sourceValue (keyword match)
        var filter = new Filter();
        filter.Must.Add(new Condition
        {
            Field = new FieldCondition
            {
                Key = "source",
                Match = new Match { Keyword = sourceValue }
            }
        });

        await _client.DeleteAsync(collectionName, filter: filter, cancellationToken: cancellationToken);
    }


    public async Task<List<VectorSearchResult>> SearchSimilarAsync(
        string collectionName,
        float[] queryVector,
        int limit = 5,
        Dictionary<string, object>? filterMetadata = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
            throw new ArgumentException("Collection name must not be null or empty.", nameof(collectionName));
        if (queryVector == null || queryVector.Length == 0)
            throw new ArgumentException("Query vector must not be null or empty.", nameof(queryVector));

        Filter? queryFilter = null;

        // Xây dựng bộ lọc từ filterMetadata
        if (filterMetadata != null && filterMetadata.Any())
        {
            var conditions = new List<Condition>();
            foreach (var kvp in filterMetadata)
            {
                if (kvp.Value == null) continue;

                if (kvp.Value is string s)
                {
                    conditions.Add(new Condition
                    {
                        Field = new FieldCondition
                        {
                            Key = kvp.Key,
                            Match = new Match { Keyword = s }
                        }
                    });
                }
                else if (kvp.Value is int i)
                {
                    conditions.Add(new Condition
                    {
                        Field = new FieldCondition
                        {
                            Key = kvp.Key,
                            Match = new Match { Integer = i }
                        }
                    });
                }
                else if (kvp.Value is long l)
                {
                    conditions.Add(new Condition
                    {
                        Field = new FieldCondition
                        {
                            Key = kvp.Key,
                            Match = new Match { Integer = l }
                        }
                    });
                }
                else if (kvp.Value is bool b)
                {
                    conditions.Add(new Condition
                    {
                        Field = new FieldCondition
                        {
                            Key = kvp.Key,
                            Match = new Match { Boolean = b }
                        }
                    });
                }
            }

            if (conditions.Any())
            {
                queryFilter = new Filter();
                queryFilter.Must.AddRange(conditions);
            }
        }

        // Thực hiện ANN search bằng Cosine Similarity (Không tạo index trong lúc search để tối ưu hiệu năng đọc)
        var searchResult = await _client.SearchAsync(
            collectionName: collectionName,
            vector: queryVector,
            filter: queryFilter,
            limit: (ulong)limit,
            cancellationToken: cancellationToken);

        return searchResult.Select(point => new VectorSearchResult
        {
            Id = !string.IsNullOrEmpty(point.Id.Uuid) ? point.Id.Uuid : point.Id.Num.ToString(),
            Score = point.Score,
            Payload = UnmapPayload(point.Payload)
        }).ToList();
    }

    #region Helper Utilities

    private static PointId MapToPointId(string id)
    {
        if (Guid.TryParse(id, out var guid))
        {
            return guid;
        }
        if (ulong.TryParse(id, out var ulongId))
        {
            return ulongId;
        }
        // Trường hợp ID là chuỗi tự do (ví dụ: "chunk-1"), sinh mã băm Deterministic UUID dùng SHA256 an toàn và hiện đại
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(id));
        var guidBytes = new byte[16];
        Array.Copy(hash, guidBytes, 16);
        return new Guid(guidBytes);
    }

    private static void MapPayload(Google.Protobuf.Collections.MapField<string, global::Qdrant.Client.Grpc.Value> target, Dictionary<string, object>? source)
    {
        if (source == null) return;

        foreach (var kvp in source)
        {
            if (kvp.Value == null) continue;

            if (kvp.Value is string s)
            {
                target[kvp.Key] = s;
            }
            else if (kvp.Value is int i)
            {
                target[kvp.Key] = i;
            }
            else if (kvp.Value is long l)
            {
                target[kvp.Key] = l;
            }
            else if (kvp.Value is float f)
            {
                target[kvp.Key] = (double)f;
            }
            else if (kvp.Value is double d)
            {
                target[kvp.Key] = d;
            }
            else if (kvp.Value is bool b)
            {
                target[kvp.Key] = b;
            }
            else if (kvp.Value is Guid g)
            {
                target[kvp.Key] = g.ToString();
            }
            else if (kvp.Value is DateTime dt)
            {
                target[kvp.Key] = dt.ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
            else if (kvp.Value is IEnumerable<string> list)
            {
                var listValue = new global::Qdrant.Client.Grpc.ListValue();
                listValue.Values.AddRange(list.Select(x => new global::Qdrant.Client.Grpc.Value { StringValue = x }));
                target[kvp.Key] = new global::Qdrant.Client.Grpc.Value { ListValue = listValue };
            }
        }
    }

    private static Dictionary<string, object> UnmapPayload(Google.Protobuf.Collections.MapField<string, global::Qdrant.Client.Grpc.Value> source)
    {
        var result = new Dictionary<string, object>();
        if (source == null) return result;

        foreach (var kvp in source)
        {
            var value = kvp.Value;
            if (value == null) continue;

            switch (value.KindCase)
            {
                case global::Qdrant.Client.Grpc.Value.KindOneofCase.StringValue:
                    result[kvp.Key] = value.StringValue;
                    break;
                case global::Qdrant.Client.Grpc.Value.KindOneofCase.IntegerValue:
                    result[kvp.Key] = value.IntegerValue;
                    break;
                case global::Qdrant.Client.Grpc.Value.KindOneofCase.DoubleValue:
                    result[kvp.Key] = value.DoubleValue;
                    break;
                case global::Qdrant.Client.Grpc.Value.KindOneofCase.BoolValue:
                    result[kvp.Key] = value.BoolValue;
                    break;
                case global::Qdrant.Client.Grpc.Value.KindOneofCase.ListValue:
                    if (value.ListValue?.Values != null)
                    {
                        result[kvp.Key] = value.ListValue.Values.Select(v => v.StringValue).ToList();
                    }
                    break;
            }
        }
        return result;
    }

    #endregion
}
