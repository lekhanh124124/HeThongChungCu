using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.Features.AI.Commands.TestLLM;

public class TestLLMCommandHandler : ICommandHandler<TestLLMCommand, TestLLMResultDto>
{
    private readonly ILLMService _llmService;
    private readonly ILogger<TestLLMCommandHandler> _logger;

    public TestLLMCommandHandler(ILLMService llmService, ILogger<TestLLMCommandHandler> logger)
    {
        _llmService = llmService;
        _logger = logger;
    }

    public async Task<Result<TestLLMResultDto>> Handle(TestLLMCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Testing LLM connection with prompt (length: {Length}).", request.Prompt.Length);

            var response = await _llmService.GenerateResponseAsync(
                prompt: request.Prompt,
                systemInstruction: "Bạn là một trợ lý ảo thân thiện.",
                cancellationToken: cancellationToken);

            _logger.LogInformation("LLM test successful. Response length: {Length} characters.", response?.Length ?? 0);

            var providerName = _llmService.GetType().Name.Contains("OpenAI", StringComparison.OrdinalIgnoreCase) ? "OpenAI" : "Gemini";

            return Result.Success(new TestLLMResultDto
            {
                Message = $"Kết nối {providerName} thành công!",
                Response = response ?? string.Empty
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LLM connection test failed.");
            var providerName = _llmService.GetType().Name.Contains("OpenAI", StringComparison.OrdinalIgnoreCase) ? "OpenAI" : "Gemini";
            return Result.Failure<TestLLMResultDto>(new Error("AI.LLM.ConnectionError", $"Kết nối {providerName} thất bại: {ex.Message}"));
        }
    }
}
