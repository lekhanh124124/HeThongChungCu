using HeThongChungCu.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Google.GenAI;
using System.Diagnostics;

namespace HeThongChungCu.Infrastructure.Embeddings;

public sealed class GeminiEmbeddingService : IEmbeddingService
{
    private const int MaxRetryAttempts = 3;
    private const int MaxInputLength = 20000;
    private static readonly TimeSpan InitialRetryDelay = TimeSpan.FromMilliseconds(500);

    private readonly Client _client;
    private readonly string _modelId;
    private readonly int _expectedVectorSize;
    private readonly ILogger<GeminiEmbeddingService> _logger;

    public GeminiEmbeddingService(
        IConfiguration configuration,
        ILogger<GeminiEmbeddingService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var apiKey = configuration["Gemini:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogCritical("Missing configuration: Gemini:ApiKey is null or empty.");
            throw new InvalidOperationException("Missing configuration: Gemini:ApiKey");
        }

        _expectedVectorSize = configuration.GetValue<int>("Gemini:EmbeddingVectorSize", 3072);
        _modelId = configuration["Gemini:EmbeddingModelId"] ?? "models/gemini-embedding-2";
        _client = new Client(apiKey: apiKey);
        
        _logger.LogInformation("GeminiEmbeddingService initialized with ModelId: {ModelId}, ExpectedVectorSize: {VectorSize}", _modelId, _expectedVectorSize);
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
            _logger.LogWarning("Embedding text length {ActualLength} exceeds maximum limit of {MaxLength}.", text.Length, MaxInputLength);
            throw new ArgumentException($"Embedding text is too long. Max length is {MaxInputLength} characters.", nameof(text));
        }

        _logger.LogDebug("Starting embedding generation for text (Length: {Length}) using model {ModelId}.", text.Length, _modelId);

        var stopwatch = Stopwatch.StartNew();
        var delay = InitialRetryDelay;

        for (int attempt = 1; attempt <= MaxRetryAttempts; attempt++)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var response = await _client.Models.EmbedContentAsync(
                    model: _modelId,
                    contents: text,
                    cancellationToken: cancellationToken);

                var embedding = response?.Embeddings?.FirstOrDefault();
                if (embedding is null || embedding.Values is null || embedding.Values.Count == 0)
                {
                    _logger.LogError("Gemini API returned empty embedding payload on attempt {Attempt}.", attempt);
                    throw new InvalidOperationException("Gemini returned an empty embedding.");
                }

                var vector = embedding.Values
                    .Select(value => (float)value)
                    .ToArray();

                if (vector.Length != _expectedVectorSize)
                {
                    _logger.LogError("Vector dimension mismatch. Expected: {Expected}, Got: {Actual}.", _expectedVectorSize, vector.Length);
                    throw new InvalidOperationException($"Unexpected embedding size. Expected {_expectedVectorSize}, got {vector.Length}.");
                }

                stopwatch.Stop();
                _logger.LogDebug(
                    "Successfully generated embedding. Vector size: {VectorSize}. Duration: {DurationMs}ms (Attempt: {Attempt}).",
                    vector.Length,
                    stopwatch.ElapsedMilliseconds,
                    attempt);

                return vector;
            }
            catch (Exception ex) when (attempt < MaxRetryAttempts && IsTransient(ex))
            {
                _logger.LogWarning(
                    ex,
                    "Transient error occurred while generating embedding on attempt {Attempt} of {MaxAttempts}. Retrying in {DelayMs}ms...",
                    attempt,
                    MaxRetryAttempts,
                    delay.TotalMilliseconds);

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
                    stopwatch.ElapsedMilliseconds,
                    _modelId,
                    ex.Message);
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
