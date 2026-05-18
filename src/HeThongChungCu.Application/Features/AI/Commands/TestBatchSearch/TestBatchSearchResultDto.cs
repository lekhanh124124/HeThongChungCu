using HeThongChungCu.Application.Common.Models;
using System.Collections.Generic;

namespace HeThongChungCu.Application.Features.AI.Commands.TestBatchSearch;

public class TestBatchSearchResultDto
{
    public string Message { get; set; } = string.Empty;
    public int TotalUpserted { get; set; }
    public List<VectorSearchResult> SearchWithoutFilter { get; set; } = new();
    public List<VectorSearchResult> SearchWithFilter { get; set; } = new();
}
