using HeThongChungCu.Application.Common.Messaging;
using System.IO;

namespace HeThongChungCu.Application.Features.AI.Commands.TestChunking;

public class TestChunkingCommand : ICommand<TestChunkingResultDto>
{
    public Stream FileStream { get; }
    public string FileName { get; }
    public long FileSize { get; }
    public int ChunkSize { get; }
    public int ChunkOverlap { get; }

    public TestChunkingCommand(Stream fileStream, string fileName, long fileSize, int chunkSize = 400, int chunkOverlap = 60)
    {
        FileStream = fileStream;
        FileName = fileName;
        FileSize = fileSize;
        ChunkSize = chunkSize;
        ChunkOverlap = chunkOverlap;
    }
}
