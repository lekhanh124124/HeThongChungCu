using Xunit;
using FluentAssertions;
using NSubstitute;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Infrastructure.OpenAI;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace HeThongChungCu.Infrastructure.IntegrationTests.OpenAI;

public class OpenAILLMServiceTests
{
    [Fact]
    public async Task GenerateResponseAsync_ShouldReturnValidText_WithConfiguredModel()
    {
        // Arrange
        var apiKey = System.Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "placeholder-key-for-testing";
        var inMemorySettings = new Dictionary<string, string?> {
            {"OpenAI:ApiKey", apiKey},
            {"OpenAI:ModelId", "gpt-4o-mini"},
            {"OpenAI:LLMTemperature", "0.2"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var logger = Substitute.For<ILogger<OpenAILLMService>>();

        var service = new OpenAILLMService(configuration, logger);

        // Act
        var response = await service.GenerateResponseAsync("Xin chào, hãy trả lời ngắn gọn trong 5 từ: 'Hệ thống chung cư'", string.Empty, string.Empty, CancellationToken.None);

        // Assert
        response.Should().NotBeNullOrWhiteSpace();
        response.Length.Should().BeGreaterThan(0);
    }
}
