using System.Collections.Generic;

namespace HeThongChungCu.Application.Common.Models;

public sealed class TextChunk
{
    public string ChunkId { get; init; } = default!;
    public string Content { get; init; } = default!;
    public int TokenCount { get; init; }
    public string Source { get; init; } = default!;
    public int ChunkIndex { get; init; }
    public string? H1 { get; init; }
    public string? H2 { get; init; }
    public string? H3 { get; init; }
    public string DocumentType { get; init; } = default!;
    public Dictionary<string, string> Metadata { get; init; } = new();
}
