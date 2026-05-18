using HeThongChungCu.Application.Common.Models;
using System.Collections.Generic;

namespace HeThongChungCu.Application.Features.AI.Commands.TestChunking;

public class TestChunkingResultDto
{
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int TotalChunks { get; set; }
    public List<TextChunk> Chunks { get; set; } = new();
}
