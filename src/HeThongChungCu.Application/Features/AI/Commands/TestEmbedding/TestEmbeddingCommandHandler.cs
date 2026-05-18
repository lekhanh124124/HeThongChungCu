using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.Features.AI.Commands.TestEmbedding;

public class TestEmbeddingCommandHandler : ICommandHandler<TestEmbeddingCommand, TestEmbeddingResultDto>
{
    private readonly IEmbeddingService _embeddingService;
    private readonly ILogger<TestEmbeddingCommandHandler> _logger;

    public TestEmbeddingCommandHandler(IEmbeddingService embeddingService, ILogger<TestEmbeddingCommandHandler> logger)
    {
        _embeddingService = embeddingService;
        _logger = logger;
    }

    public async Task<Result<TestEmbeddingResultDto>> Handle(TestEmbeddingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Testing embedding generation for text (length: {Length}).", request.Text.Length);

            var vector = await _embeddingService.GenerateEmbeddingAsync(request.Text, cancellationToken);

            _logger.LogInformation("Embedding test successful. Vector size: {Size}.", vector.Length);

            return Result.Success(new TestEmbeddingResultDto
            {
                Message = "Sinh embedding thành công!",
                VectorSize = vector.Length,
                Preview = vector.Take(10).ToArray()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Embedding generation test failed.");
            return Result.Failure<TestEmbeddingResultDto>(new Error("AI.Embedding.Error", $"Sinh embedding thất bại: {ex.Message}"));
        }
    }
}
