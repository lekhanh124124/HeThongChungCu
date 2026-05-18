using Xunit;
using FluentAssertions;
using NSubstitute;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Infrastructure.Embeddings;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace HeThongChungCu.Infrastructure.IntegrationTests.Chunking;

public class GeminiEmbeddingServiceTests
{
    [Fact]
    public async Task GenerateEmbeddingAsync_ShouldReturnValidVector_WithConfiguredModel()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?> {
            {"Gemini:ApiKey", "AIzaSyCBUNEUzePMsbgUB1LK_9gGdlNqnmhT3ng"},
            {"Gemini:EmbeddingModelId", "models/gemini-embedding-2"},
            {"Gemini:EmbeddingVectorSize", "3072"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var logger = Substitute.For<ILogger<GeminiEmbeddingService>>();

        var service = new GeminiEmbeddingService(configuration, logger);

        // Act
        var vector = await service.GenerateEmbeddingAsync("Hệ thống quản lý chung cư thông minh", CancellationToken.None);

        // Assert
        vector.Should().NotBeNull();
        vector.Length.Should().Be(3072);
    }
}
