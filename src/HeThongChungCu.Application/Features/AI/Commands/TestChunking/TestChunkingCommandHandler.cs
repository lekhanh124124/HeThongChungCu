using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.Features.AI.Commands.TestChunking;

public class TestChunkingCommandHandler : ICommandHandler<TestChunkingCommand, TestChunkingResultDto>
{
    private readonly ITextChunker _textChunker;
    private readonly ILogger<TestChunkingCommandHandler> _logger;

    public TestChunkingCommandHandler(ITextChunker textChunker, ILogger<TestChunkingCommandHandler> logger)
    {
        _textChunker = textChunker;
        _logger = logger;
    }

    public async Task<Result<TestChunkingResultDto>> Handle(TestChunkingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Testing text chunking for file: {FileName} (ChunkSize: {ChunkSize}, Overlap: {Overlap})",
                request.FileName, request.ChunkSize, request.ChunkOverlap);

            using var reader = new StreamReader(request.FileStream);
            var content = await reader.ReadToEndAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(content))
            {
                return Result.Failure<TestChunkingResultDto>(new Error("AI.Chunking.EmptyContent", "Nội dung file không được để trống."));
            }

            var chunks = _textChunker.SplitText(content, request.FileName, request.ChunkSize, request.ChunkOverlap);

            _logger.LogInformation("Chunking completed: {TotalChunks} chunks generated from {FileName}.", chunks.Count, request.FileName);

            return Result.Success(new TestChunkingResultDto
            {
                FileName = request.FileName,
                FileSize = request.FileSize,
                TotalChunks = chunks.Count,
                Chunks = chunks
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during chunking test for file: {FileName}.", request.FileName);
            return Result.Failure<TestChunkingResultDto>(new Error("AI.Chunking.Error", $"Lỗi khi chia nhỏ văn bản: {ex.Message}"));
        }
    }
}
