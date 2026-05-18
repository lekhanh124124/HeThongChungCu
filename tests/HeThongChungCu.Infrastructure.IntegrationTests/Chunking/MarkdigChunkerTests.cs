using FluentAssertions;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Infrastructure.Chunking;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace HeThongChungCu.Infrastructure.IntegrationTests.Chunking;

public class MarkdigChunkerTests
{
    private readonly ITestOutputHelper _output;

    public MarkdigChunkerTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void SplitText_ShouldCorrectlyChunkMarkdownFileWithAdvancedRAGRules()
    {
        // Arrange
        var chunker = new MarkdigChunker();
        var filePath = Path.Combine("..", "..", "..", "..", "..", "knowledge-base", "meta", "huong-dan-tra-loi-danh-cho-ai.md");
        filePath = Path.GetFullPath(filePath);

        File.Exists(filePath).Should().BeTrue($"File should exist at path: {filePath}");

        var markdownContent = File.ReadAllText(filePath);
        var sourceName = Path.GetFileName(filePath);
        int chunkSize = 400;
        int overlap = 60;
        int minTokensLimit = 100;

        // Act
        var chunks = chunker.SplitText(markdownContent, sourceName, chunkSize, overlap);

        // Assert
        chunks.Should().NotBeEmpty();
        _output.WriteLine($"Total Chunks: {chunks.Count}");

        var chunkIds = chunks.Select(c => c.ChunkId).ToList();
        chunkIds.Should().OnlyHaveUniqueItems("Every chunk must have a unique stable ID.");

        for (int i = 0; i < chunks.Count; i++)
        {
            var chunk = chunks[i];
            _output.WriteLine($"--- CHUNK {i} (Stable ID: {chunk.ChunkId}) ---");
            _output.WriteLine($"Token Count: {chunk.TokenCount}, Char Length: {chunk.Content.Length}");
            _output.WriteLine($"Source: {chunk.Source}");
            _output.WriteLine($"DocumentType: {chunk.DocumentType}");
            _output.WriteLine($"H1: {chunk.H1}");
            _output.WriteLine($"H2: {chunk.H2}");
            _output.WriteLine($"H3: {chunk.H3}");
            _output.WriteLine("Metadata:");
            foreach (var kvp in chunk.Metadata)
            {
                _output.WriteLine($"  {kvp.Key}: {kvp.Value}");
            }
            _output.WriteLine("Content Preview:");
            _output.WriteLine(chunk.Content);
            _output.WriteLine(new string('-', 50));

            // Tiêu chuẩn chất lượng nghiêm ngặt cho RAG Production:
            
            // 1. Không chunk nào được rỗng
            chunk.Content.Should().NotBeNullOrWhiteSpace();

            // 2. Định danh chunk index phải chính xác
            chunk.ChunkIndex.Should().Be(i);

            // 3. Nguồn tài liệu phải đúng
            chunk.Source.Should().Be(sourceName);

            // 4. YAML Frontmatter không được chứa trong phần thân Content
            chunk.Content.Should().NotContain("module: HeThongChungCu");
            chunk.Content.Should().NotStartWith("---");

            // 5. Metadata phân cấp phải khớp
            chunk.Metadata.Should().ContainKey("module").WhoseValue.Should().Be("HeThongChungCu");
            chunk.Metadata.Should().ContainKey("feature").WhoseValue.Should().Be("TroLyAoCuDan");
            chunk.Metadata.Should().ContainKey("document_type").WhoseValue.Should().Be("policy");
            chunk.Metadata.Should().ContainKey("audience").WhoseValue.Should().Be("resident");

            // 6. Stable ID phải đúng định dạng và có trong metadata
            chunk.ChunkId.Should().StartWith("chunk_huongdantraloi");
            chunk.Metadata.Should().ContainKey("chunk_id").WhoseValue.Should().Be(chunk.ChunkId);

            // 7. KIỂM SOÁT TOKEN CỰC KỲ KHẮT KHE (Triệt tiêu hoàn toàn lỗi chunk khổng lồ)
            // Không chunk nào được vượt quá chunkSize + biên nhỏ cho breadcrumbs (e.g. 450 tokens)
            chunk.TokenCount.Should().BeLessThanOrEqualTo(chunkSize + 80, 
                $"Chunk {i} must be within maximum size limit (found {chunk.TokenCount} tokens).");

            // 8. TRÁNH CHUNK NHỎ VÔ DỤNG (Min Chunk tokens)
            // Đảm bảo mọi chunk đều >= 100 tokens nhờ thuật toán Tiny Chunk Merging
            chunk.TokenCount.Should().BeGreaterThanOrEqualTo(minTokensLimit, 
                $"Chunk {i} must satisfy the minimum token limit of 100 tokens (found {chunk.TokenCount} tokens).");
        }
    }
}
