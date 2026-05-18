using System.Collections.Generic;

namespace HeThongChungCu.Application.Common.Models;

public class VectorSearchResult
{
    public string Id { get; set; } = string.Empty;
    public float Score { get; set; }
    public Dictionary<string, object> Payload { get; set; } = new();
}
