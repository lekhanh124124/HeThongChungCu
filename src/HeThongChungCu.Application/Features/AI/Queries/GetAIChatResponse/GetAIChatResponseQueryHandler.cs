using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.Features.AI.Queries.GetAIChatResponse;

public class GetAIChatResponseQueryHandler : IQueryHandler<GetAIChatResponseQuery, AIChatResponseDto>
{
    private readonly IVectorStore _vectorStore;
    private readonly IEmbeddingService _embeddingService;
    private readonly ILLMService _llmService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GetAIChatResponseQueryHandler> _logger;

    public GetAIChatResponseQueryHandler(
        IVectorStore vectorStore,
        IEmbeddingService embeddingService,
        ILLMService llmService,
        IConfiguration configuration,
        ILogger<GetAIChatResponseQueryHandler> logger)
    {
        _vectorStore = vectorStore;
        _embeddingService = embeddingService;
        _llmService = llmService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Result<AIChatResponseDto>> Handle(GetAIChatResponseQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
            {
                return Result.Failure<AIChatResponseDto>(new Error("AI.InvalidPrompt", "Câu hỏi không được để trống."));
            }

            var hasHistory = request.History != null && request.History.Count > 0;

            _logger.LogInformation(
                "Processing RAG chatbot request. Prompt: '{Prompt}' | History: {HistoryCount} turns | DocumentType: {Type}",
                request.Prompt.Length > 60 ? request.Prompt[..60] + "..." : request.Prompt,
                request.History?.Count ?? 0,
                request.DocumentType ?? "None");

            // ═══════════════════════════════════════════════════════════════════
            // BƯỚC 1 – CONDENSE (chỉ thực hiện khi có lịch sử hội thoại)
            // Dùng LLM viết lại câu hỏi thành Standalone Question đầy đủ ngữ cảnh.
            // ═══════════════════════════════════════════════════════════════════
            string searchQuery;
            bool isCondensed = false;

            if (hasHistory)
            {
                _logger.LogDebug("Step 1/3 – CONDENSE: Condensing question with {Count} history turns...", request.History!.Count);

                searchQuery = await _llmService.CondenseQuestionAsync(
                    currentQuestion: request.Prompt,
                    history: request.History,
                    cancellationToken: cancellationToken);

                isCondensed = !string.Equals(searchQuery, request.Prompt, StringComparison.OrdinalIgnoreCase);

                _logger.LogInformation("Step 1/3 – CONDENSE complete. IsCondensed: {IsCondensed}", isCondensed);
            }
            else
            {
                // Single-turn: không cần condense, dùng prompt gốc trực tiếp
                searchQuery = request.Prompt;
                _logger.LogDebug("Step 1/3 – CONDENSE skipped (no history). Using original prompt for search.");
            }

            // ═══════════════════════════════════════════════════════════════════
            // BƯỚC 2 – RETRIEVE: Embedding → Qdrant Search
            // Dùng Standalone Question để sinh embedding và tìm kiếm vector.
            // ═══════════════════════════════════════════════════════════════════
            _logger.LogDebug("Step 2/3 – RETRIEVE: Generating embedding for query...");
            var queryVector = await _embeddingService.GenerateEmbeddingAsync(searchQuery, cancellationToken);

            var modelId = _embeddingService.ModelId;
            var collectionName = _configuration[$"Qdrant:Collections:{modelId}"];
            if (string.IsNullOrWhiteSpace(collectionName))
            {
                collectionName = _configuration["Qdrant:CollectionName"] ?? "resident_knowledge_base";
            }

            var filterMetadata = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(request.DocumentType))
            {
                filterMetadata.Add("document_type", request.DocumentType);
            }

            _logger.LogDebug("Step 2/3 – RETRIEVE: Searching Qdrant collection '{Collection}'...", collectionName);
            var searchResults = await _vectorStore.SearchSimilarAsync(
                collectionName: collectionName,
                queryVector: queryVector,
                limit: request.Limit,
                filterMetadata: filterMetadata.Any() ? filterMetadata : null,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Step 2/3 – RETRIEVE complete. Found {Count} matching chunks.", searchResults?.Count ?? 0);

            // ═══════════════════════════════════════════════════════════════════
            // BƯỚC 3 – GENERATE: Lắp ghép ngữ cảnh + Lịch sử → LLM → Câu trả lời
            // ═══════════════════════════════════════════════════════════════════
            var contextBuilder = new StringBuilder();
            var citationList = new List<AIChatSourceDto>();

            if (searchResults != null && searchResults.Any())
            {
                foreach (var result in searchResults)
                {
                    var payload = result.Payload;
                    if (payload == null) continue;

                    var source = payload.TryGetValue("source", out var srcVal) ? srcVal.ToString() : "Chưa rõ";
                    var content = payload.TryGetValue("content", out var cntVal) ? cntVal.ToString() : string.Empty;
                    var h1 = payload.TryGetValue("h1", out var h1Val) ? h1Val.ToString() : string.Empty;
                    var h2 = payload.TryGetValue("h2", out var h2Val) ? h2Val.ToString() : string.Empty;
                    var h3 = payload.TryGetValue("h3", out var h3Val) ? h3Val.ToString() : string.Empty;

                    contextBuilder.AppendLine($"### NGUỒN TÀI LIỆU: {source}");

                    var headers = new List<string>();
                    if (!string.IsNullOrEmpty(h1)) headers.Add(h1);
                    if (!string.IsNullOrEmpty(h2)) headers.Add(h2);
                    if (!string.IsNullOrEmpty(h3)) headers.Add(h3);

                    if (headers.Any())
                    {
                        contextBuilder.AppendLine($"Mục: {string.Join(" > ", headers)}");
                    }
                    contextBuilder.AppendLine();
                    contextBuilder.AppendLine(content);
                    contextBuilder.AppendLine();
                    contextBuilder.AppendLine("---");
                    contextBuilder.AppendLine();

                    citationList.Add(new AIChatSourceDto
                    {
                        Source = source ?? "Chưa rõ",
                        H1 = h1 ?? string.Empty,
                        H2 = h2 ?? string.Empty,
                        H3 = h3 ?? string.Empty,
                        Score = result.Score
                    });
                }
            }

            // Bổ sung lịch sử hội thoại gần nhất (sliding window 5 lượt) vào ngữ cảnh cho LLM
            var historyContext = BuildHistoryContext(request.History);

            _logger.LogDebug(
                "Step 3/3 – GENERATE: Invoking LLM (Context: {ContextLen} chars, History: {HistoryLen} chars)...",
                contextBuilder.Length, historyContext.Length);

            var fullContext = string.IsNullOrEmpty(historyContext)
                ? contextBuilder.ToString()
                : $"{contextBuilder}\n\n{historyContext}";

            var answer = await _llmService.GenerateResponseAsync(
                prompt: request.Prompt,
                context: fullContext,
                systemInstruction: "Hãy luôn thể hiện tinh thần phục vụ chu đáo, hỗ trợ tận tâm với cư dân.",
                cancellationToken: cancellationToken);

            _logger.LogInformation(
                "Step 3/3 – GENERATE complete. Answer: {Length} chars. IsCondensed: {IsCondensed}.",
                answer?.Length ?? 0, isCondensed);

            return Result.Success(new AIChatResponseDto
            {
                Answer = answer ?? string.Empty,
                Sources = citationList,
                IsCondensed = isCondensed
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute GetAIChatResponseQuery.");
            return Result.Failure<AIChatResponseDto>(new Error("AI.ChatError", $"Lỗi hội thoại trợ lý ảo: {ex.Message}"));
        }
    }

    /// <summary>
    /// Xây dựng phần lịch sử hội thoại được thêm vào context cho LLM,
    /// áp dụng sliding window 5 lượt cuối.
    /// </summary>
    private static string BuildHistoryContext(List<ChatMessageDto>? history)
    {
        if (history == null || !history.Any()) return string.Empty;

        var recentHistory = history.TakeLast(5).ToList();
        var sb = new StringBuilder();
        sb.AppendLine("[LỊCH SỬ HỘI THOẠI GẦN NHẤT]");

        foreach (var msg in recentHistory)
        {
            var roleLabel = msg.Role.Equals("user", StringComparison.OrdinalIgnoreCase) ? "Cư dân" : "Trợ lý";
            sb.AppendLine($"{roleLabel}: {msg.Content}");
        }

        return sb.ToString();
    }
}
