using HeThongChungCu.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Embeddings;
using System.Diagnostics;

namespace HeThongChungCu.Infrastructure.OpenAI;

public sealed class OpenAIEmbeddingService : IEmbeddingService
{
    private const int MaxRetryAttempts = 3;
    private const int MaxInputLength = 8191; // OpenAI text-embedding-3-small token limit (chars approx)
    private static readonly TimeSpan InitialRetryDelay = TimeSpan.FromMilliseconds(500);

    private readonly EmbeddingClient _client;
    private readonly string _modelId;
    private readonly int _expectedVectorSize;
    private readonly ILogger<OpenAIEmbeddingService> _logger;

    public OpenAIEmbeddingService(
        IConfiguration configuration,
        ILogger<OpenAIEmbeddingService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var apiKey = configuration["OpenAI:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogCritical("Missing configuration: OpenAI:ApiKey is null or empty.");
            throw new InvalidOperationException("Missing configuration: OpenAI:ApiKey");
        }

        _modelId = configuration["OpenAI:EmbeddingModelId"] ?? "text-embedding-3-small";
        _expectedVectorSize = configuration.GetValue<int>("OpenAI:EmbeddingVectorSize", 1536);

        _client = new EmbeddingClient(model: _modelId, apiKey: apiKey);

        _logger.LogInformation(
            "OpenAIEmbeddingService initialized with ModelId: {ModelId}, ExpectedVectorSize: {VectorSize}",
            _modelId, _expectedVectorSize);
    }

    public string ModelId => _modelId;

    public async Task<float[]> GenerateEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            _logger.LogWarning("GenerateEmbeddingAsync received empty or null text input.");
            throw new ArgumentException("Embedding text must not be empty.", nameof(text));
        }

        if (text.Length > MaxInputLength)
        {
            _logger.LogWarning(
                "Embedding text length {ActualLength} exceeds maximum limit of {MaxLength}. Truncating.",
                text.Length, MaxInputLength);
            text = text[..MaxInputLength];
        }

        _logger.LogDebug(
            "Starting embedding generation for text (Length: {Length}) using model {ModelId}.",
            text.Length, _modelId);

        var stopwatch = Stopwatch.StartNew();
        var delay = InitialRetryDelay;

        for (int attempt = 1; attempt <= MaxRetryAttempts; attempt++)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var response = await _client.GenerateEmbeddingAsync(
                    input: text,
                    cancellationToken: cancellationToken);

                var embedding = response?.Value;
                if (embedding is null)
                {
                    _logger.LogError("OpenAI API returned null embedding payload on attempt {Attempt}.", attempt);
                    throw new InvalidOperationException("OpenAI returned a null embedding.");
                }

                var vector = embedding.ToFloats().ToArray();

                if (vector.Length != _expectedVectorSize)
                {
                    _logger.LogError(
                        "Vector dimension mismatch. Expected: {Expected}, Got: {Actual}.",
                        _expectedVectorSize, vector.Length);
                    throw new InvalidOperationException(
                        $"Unexpected embedding size. Expected {_expectedVectorSize}, got {vector.Length}.");
                }

                stopwatch.Stop();
                _logger.LogDebug(
                    "Successfully generated embedding. Vector size: {VectorSize}. Duration: {DurationMs}ms (Attempt: {Attempt}).",
                    vector.Length, stopwatch.ElapsedMilliseconds, attempt);

                return vector;
            }
            catch (Exception ex) when (attempt < MaxRetryAttempts && IsTransient(ex))
            {
                _logger.LogWarning(
                    ex,
                    "Transient error occurred while generating embedding on attempt {Attempt} of {MaxAttempts}. Retrying in {DelayMs}ms...",
                    attempt, MaxRetryAttempts, delay.TotalMilliseconds);

                await Task.Delay(delay, cancellationToken);
                delay *= 2; // Exponential backoff
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Embedding generation request was canceled after {DurationMs}ms.", stopwatch.ElapsedMilliseconds);
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(
                    ex,
                    "Failed to generate embedding after {DurationMs}ms. Model: {ModelId}. Error: {Error}",
                    stopwatch.ElapsedMilliseconds, _modelId, ex.Message);
                throw;
            }
        }

        throw new InvalidOperationException("Failed to generate embedding due to transient errors.");
    }

    private static bool IsTransient(Exception ex)
    {
        var message = ex.Message.ToLowerInvariant();
        return ex is HttpRequestException
            || ex is TimeoutException
            || message.Contains("429")
            || message.Contains("rate limit")
            || message.Contains("500")
            || message.Contains("502")
            || message.Contains("503")
            || message.Contains("504")
            || message.Contains("unavailable")
            || message.Contains("deadline exceeded");
    }
}
