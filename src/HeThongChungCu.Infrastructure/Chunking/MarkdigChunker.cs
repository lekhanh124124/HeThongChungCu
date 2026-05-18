using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Models;
using Markdig;
using Markdig.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeThongChungCu.Infrastructure.Chunking;

public class MarkdigChunker : ITextChunker
{
    public List<TextChunk> SplitText(string text, string source = "", int chunkSize = 400, int chunkOverlap = 60)
    {
        if (string.IsNullOrWhiteSpace(text)) return new List<TextChunk>();

        // Bước 1: Trích xuất và bóc tách YAML frontmatter
        var (parsedMetadata, cleanText) = MarkdownChunkingHelper.ParseFrontmatter(text);

        var pipeline = new MarkdownPipelineBuilder().Build();
        var document = Markdown.Parse(cleanText, pipeline);

        var blocks = new List<MarkdownBlock>();
        var activeHeaders = new string?[7]; // 1-indexed (levels 1 to 6)

        // Bước 2: Phân tích tài liệu thành các block và làm phẳng List trong AST
        foreach (var block in document)
        {
            var blockText = cleanText.Substring(block.Span.Start, block.Span.Length);
            
            // Lấy Active Headers hiện tại
            var parentHeaders = new List<string>();
            for (int h = 1; h <= 6; h++)
            {
                if (activeHeaders[h] != null)
                {
                    parentHeaders.Add(activeHeaders[h]!);
                }
            }

            if (block is HeadingBlock heading)
            {
                int level = heading.Level;
                activeHeaders[level] = blockText;
                
                // Clear all sub-headers
                for (int i = level + 1; i <= 6; i++)
                {
                    activeHeaders[i] = null;
                }

                var cleanParentHeaders = new List<string>();
                for (int i = 1; i < level; i++)
                {
                    if (activeHeaders[i] != null)
                    {
                        cleanParentHeaders.Add(activeHeaders[i]!);
                    }
                }

                blocks.Add(new MarkdownBlock
                {
                    Text = blockText,
                    IsHeading = true,
                    HeadingLevel = level,
                    ActiveHeaders = cleanParentHeaders
                });
            }
            else if (block is ListBlock listBlock)
            {
                // Làm phẳng danh sách thành các list item độc lập (Level 2 Block)
                foreach (var item in listBlock)
                {
                    if (item is ListItemBlock listItem)
                    {
                        var itemText = cleanText.Substring(listItem.Span.Start, listItem.Span.Length);
                        int itemTokens = MarkdownChunkingHelper.GetTokenCount(itemText);
                        
                        if (itemTokens > chunkSize)
                        {
                            var splitItems = MarkdownChunkingHelper.RecursiveSplitBlock(itemText, chunkSize, parentHeaders);
                            blocks.AddRange(splitItems);
                        }
                        else
                        {
                            blocks.Add(new MarkdownBlock
                            {
                                Text = itemText,
                                IsHeading = false,
                                ActiveHeaders = parentHeaders
                            });
                        }
                    }
                }
            }
            else
            {
                int blockTokens = MarkdownChunkingHelper.GetTokenCount(blockText);
                if (blockTokens > chunkSize)
                {
                    var splitBlocks = MarkdownChunkingHelper.RecursiveSplitBlock(blockText, chunkSize, parentHeaders);
                    blocks.AddRange(splitBlocks);
                }
                else
                {
                    blocks.Add(new MarkdownBlock
                    {
                        Text = blockText,
                        IsHeading = false,
                        ActiveHeaders = parentHeaders
                    });
                }
            }
        }

        var chunks = new List<TextChunk>();
        var currentChunkBlocks = new List<MarkdownBlock>();
        string lastChunkOverlap = string.Empty;

        // Bước 3: Phân nhóm các block vào các chunk và áp dụng thuật toán ngắt nâng cao
        for (int i = 0; i < blocks.Count; i++)
        {
            var block = blocks[i];

            if (currentChunkBlocks.Count > 0)
            {
                // A. Heading-Aware Hard Boundary (Phân ranh giới cứng theo Tiêu đề mới)
                // Chỉ ngắt chunk nếu số token hiện tại đã lớn hơn MinChunkTokens (100)
                if (block.IsHeading && block.HeadingLevel >= 2)
                {
                    var currentFormatted = FormatChunkContent(currentChunkBlocks, lastChunkOverlap);
                    int currentTokens = MarkdownChunkingHelper.GetTokenCount(currentFormatted);
                    
                    if (currentTokens >= 100)
                    {
                        chunks.Add(BuildTextChunk(currentFormatted, currentChunkBlocks, chunks.Count, source, parsedMetadata));

                        lastChunkOverlap = MarkdownChunkingHelper.GetSentenceOverlap(currentChunkBlocks, chunkOverlap);
                        currentChunkBlocks = new List<MarkdownBlock>();
                    }
                }

                // B. Kiểm tra tràn kích thước chunk tối đa (chunkSize)
                var tempBlocks = new List<MarkdownBlock>(currentChunkBlocks) { block };
                var formattedTemp = FormatChunkContent(tempBlocks, lastChunkOverlap);
                int tokenCount = MarkdownChunkingHelper.GetTokenCount(formattedTemp);

                if (tokenCount > chunkSize)
                {
                    var currentFormatted = FormatChunkContent(currentChunkBlocks, lastChunkOverlap);
                    int currentTokens = MarkdownChunkingHelper.GetTokenCount(currentFormatted);

                    // Chỉ ngắt chunk nếu số token hiện tại đạt ngưỡng tối thiểu 100 HOẶC nếu gộp chung quá lớn (> chunkSize + 150)
                    if (currentTokens >= 100 || tokenCount > chunkSize + 150)
                    {
                        // Tối ưu hóa: Tránh tiêu đề mồ côi (Orphan Heading) ở cuối chunk hiện tại
                        var blocksToSave = new List<MarkdownBlock>(currentChunkBlocks);
                        var trailingHeadings = new List<MarkdownBlock>();

                        while (blocksToSave.Count > 0 && blocksToSave.Last().IsHeading)
                        {
                            var lastHeading = blocksToSave.Last();
                            blocksToSave.RemoveAt(blocksToSave.Count - 1);
                            trailingHeadings.Insert(0, lastHeading);
                        }

                        if (blocksToSave.Count > 0)
                        {
                            var finalChunkContent = FormatChunkContent(blocksToSave, lastChunkOverlap);
                            chunks.Add(BuildTextChunk(finalChunkContent, blocksToSave, chunks.Count, source, parsedMetadata));

                            lastChunkOverlap = MarkdownChunkingHelper.GetSentenceOverlap(blocksToSave, chunkOverlap);
                            currentChunkBlocks = new List<MarkdownBlock>();

                            // Đưa các heading bị loại bỏ vào đầu chunk tiếp theo
                            foreach (var heading in trailingHeadings)
                            {
                                currentChunkBlocks.Add(heading);
                            }
                        }
                        else
                        {
                            var finalChunkContent = FormatChunkContent(currentChunkBlocks, lastChunkOverlap);
                            chunks.Add(BuildTextChunk(finalChunkContent, currentChunkBlocks, chunks.Count, source, parsedMetadata));
                            
                            lastChunkOverlap = MarkdownChunkingHelper.GetSentenceOverlap(currentChunkBlocks, chunkOverlap);
                            currentChunkBlocks = new List<MarkdownBlock>();
                        }
                    }
                }
            }

            // Chỉ thêm block vào chunk hiện tại nếu nó chưa tồn tại (đề phòng trùng lặp khi quay vòng)
            if (!currentChunkBlocks.Any(b => b.Text == block.Text && b.IsHeading == block.IsHeading))
            {
                currentChunkBlocks.Add(block);
            }
        }

        // Thêm chunk cuối cùng nếu còn block
        if (currentChunkBlocks.Count > 0)
        {
            var finalChunkContent = FormatChunkContent(currentChunkBlocks, lastChunkOverlap);
            int finalTokens = MarkdownChunkingHelper.GetTokenCount(finalChunkContent);

            // Tối ưu hóa cuối cùng: Merge chunk cuối nếu nó quá nhỏ (< 100 tokens) để tránh chunk rác
            if (finalTokens < 100 && chunks.Count > 0)
            {
                var lastChunk = chunks[^1];
                var mergedContent = $"{lastChunk.Content}\n\n{finalChunkContent}".Trim();
                
                var mergedIndex = lastChunk.ChunkIndex;
                var mergedId = MarkdownChunkingHelper.GenerateStableId(source, mergedIndex, mergedContent);

                chunks[^1] = new TextChunk
                {
                    ChunkId = mergedId,
                    Content = mergedContent,
                    TokenCount = MarkdownChunkingHelper.GetTokenCount(mergedContent),
                    Source = lastChunk.Source,
                    ChunkIndex = mergedIndex,
                    H1 = lastChunk.H1 ?? GetActiveHeaderAtLevel(currentChunkBlocks[0], currentChunkBlocks, 1),
                    H2 = lastChunk.H2 ?? GetActiveHeaderAtLevel(currentChunkBlocks[0], currentChunkBlocks, 2),
                    H3 = lastChunk.H3 ?? GetActiveHeaderAtLevel(currentChunkBlocks[0], currentChunkBlocks, 3),
                    DocumentType = lastChunk.DocumentType,
                    Metadata = new Dictionary<string, string>(lastChunk.Metadata)
                    {
                        ["chunk_id"] = mergedId,
                        ["chunk_index"] = mergedIndex.ToString()
                    }
                };
            }
            else
            {
                chunks.Add(BuildTextChunk(finalChunkContent, currentChunkBlocks, chunks.Count, source, parsedMetadata));
            }
        }

        return chunks;
    }

    private string FormatChunkContent(List<MarkdownBlock> blocks, string sentenceOverlap = "")
    {
        if (blocks.Count == 0) return string.Empty;

        var firstBlock = blocks[0];
        var contextHeaders = firstBlock.ActiveHeaders;

        var sb = new StringBuilder();
        var activeParentHeaders = new List<string>();
        
        foreach (var h in contextHeaders)
        {
            if (blocks.Any(b => b.IsHeading && b.Text == h))
                continue;

            var cleanH = h.TrimStart('#').Trim();
            activeParentHeaders.Add(cleanH);
        }

        if (activeParentHeaders.Count > 0)
        {
            sb.AppendLine($"[{string.Join(" > ", activeParentHeaders)}]");
            sb.AppendLine();
        }

        if (!string.IsNullOrWhiteSpace(sentenceOverlap))
        {
            sb.AppendLine(sentenceOverlap);
            sb.AppendLine();
        }

        foreach (var b in blocks)
        {
            sb.AppendLine(b.Text);
            sb.AppendLine();
        }

        return sb.ToString().Trim();
    }

    private TextChunk BuildTextChunk(
        string content, 
        List<MarkdownBlock> blocks, 
        int index, 
        string source, 
        Dictionary<string, string> frontmatter)
    {
        string? h1 = null;
        string? h2 = null;
        string? h3 = null;

        if (blocks.Count > 0)
        {
            var firstBlock = blocks[0];
            h1 = GetActiveHeaderAtLevel(firstBlock, blocks, 1);
            h2 = GetActiveHeaderAtLevel(firstBlock, blocks, 2);
            h3 = GetActiveHeaderAtLevel(firstBlock, blocks, 3);
        }

        var metadata = new Dictionary<string, string>(frontmatter);
        if (h1 != null) metadata["h1"] = h1;
        if (h2 != null) metadata["h2"] = h2;
        if (h3 != null) metadata["h3"] = h3;
        metadata["source"] = source;
        metadata["chunk_index"] = index.ToString();

        var docType = frontmatter.TryGetValue("document_type", out var type) ? type : "general";
        
        var chunkId = MarkdownChunkingHelper.GenerateStableId(source, index, content);
        metadata["chunk_id"] = chunkId;

        return new TextChunk
        {
            ChunkId = chunkId,
            Content = content,
            TokenCount = MarkdownChunkingHelper.GetTokenCount(content),
            Source = source,
            ChunkIndex = index,
            H1 = h1,
            H2 = h2,
            H3 = h3,
            DocumentType = docType,
            Metadata = metadata
        };
    }

    private string? GetActiveHeaderAtLevel(MarkdownBlock firstBlock, List<MarkdownBlock> blocks, int level)
    {
        var bodyHeading = blocks.FirstOrDefault(b => b.IsHeading && b.HeadingLevel == level);
        if (bodyHeading != null)
        {
            return bodyHeading.Text.TrimStart('#').Trim();
        }

        foreach (var h in firstBlock.ActiveHeaders)
        {
            int hLevel = h.TakeWhile(c => c == '#').Count();
            if (hLevel == level)
            {
                return h.TrimStart('#').Trim();
            }
        }

        return null;
    }
}
