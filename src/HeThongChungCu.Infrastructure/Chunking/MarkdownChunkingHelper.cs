using SharpToken;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace HeThongChungCu.Infrastructure.Chunking;

internal static class MarkdownChunkingHelper
{
    private static readonly GptEncoding _encoding = GptEncoding.GetEncoding("cl100k_base");

    public static int GetTokenCount(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0;
        return _encoding.Encode(text).Count;
    }

    public static (Dictionary<string, string> Metadata, string Content) ParseFrontmatter(string text)
    {
        var metadata = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(text)) return (metadata, text);

        var normalizedText = text.Replace("\r\n", "\n");
        var lines = normalizedText.Split('\n');

        if (lines.Length > 0 && lines[0].Trim() == "---")
        {
            int closingIndex = -1;
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].Trim() == "---")
                {
                    closingIndex = i;
                    break;
                }
            }

            if (closingIndex > 0)
            {
                for (int i = 1; i < closingIndex; i++)
                {
                    var line = lines[i].Trim();
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#')) continue;

                    int colonIndex = line.IndexOf(':');
                    if (colonIndex > 0)
                    {
                        var key = line.Substring(0, colonIndex).Trim();
                        var val = line.Substring(colonIndex + 1).Trim();

                        if (val.StartsWith('[') && val.EndsWith(']'))
                        {
                            val = val.Substring(1, val.Length - 2).Trim();
                        }
                        else if ((val.StartsWith('"') && val.EndsWith('"')) || (val.StartsWith('\'') && val.EndsWith('\'')))
                        {
                            val = val.Substring(1, val.Length - 2).Trim();
                        }

                        metadata[key] = val;
                    }
                }

                var remainingLines = lines.Skip(closingIndex + 1);
                var cleanContent = string.Join("\n", remainingLines).Trim();
                return (metadata, cleanContent);
            }
        }

        return (metadata, text);
    }

    public static string GenerateStableId(string source, int index, string content)
    {
        using var md5 = MD5.Create();
        var inputBytes = Encoding.UTF8.GetBytes($"{source}_{index}_{content}");
        var hashBytes = md5.ComputeHash(inputBytes);
        var sb = new StringBuilder();
        foreach (var b in hashBytes.Take(8))
        {
            sb.Append(b.ToString("x2"));
        }
        
        var cleanSource = new string(source.Where(char.IsLetterOrDigit).ToArray()).ToLower();
        if (cleanSource.Length > 20) cleanSource = cleanSource.Substring(0, 20);
        
        return $"chunk_{cleanSource}_{index}_{sb}";
    }

    public static List<MarkdownBlock> RecursiveSplitBlock(string text, int chunkSize, List<string> activeHeaders)
    {
        var subBlocks = new List<MarkdownBlock>();
        if (string.IsNullOrWhiteSpace(text)) return subBlocks;

        // Level 3: Sentence Split
        var sentences = Regex.Split(text, @"(?<=[.!?])\s+");
        var currentSubText = new StringBuilder();

        foreach (var sentence in sentences)
        {
            if (string.IsNullOrWhiteSpace(sentence)) continue;

            var tempText = currentSubText.Length == 0 ? sentence : $"{currentSubText} {sentence}";
            int tempTokens = GetTokenCount(tempText);

            if (tempTokens > chunkSize)
            {
                if (currentSubText.Length > 0)
                {
                    subBlocks.Add(new MarkdownBlock
                    {
                        Text = currentSubText.ToString().Trim(),
                        IsHeading = false,
                        ActiveHeaders = activeHeaders
                    });
                    currentSubText.Clear();
                }

                // Level 4: Hard Token Split (Fallback nếu 1 câu đơn lẻ vẫn > chunkSize)
                int sentenceTokens = GetTokenCount(sentence);
                if (sentenceTokens > chunkSize)
                {
                    var words = sentence.Split(' ');
                    var currentWordText = new StringBuilder();

                    foreach (var word in words)
                    {
                        var wordTemp = currentWordText.Length == 0 ? word : $"{currentWordText} {word}";
                        if (GetTokenCount(wordTemp) > chunkSize)
                        {
                            if (currentWordText.Length > 0)
                            {
                                subBlocks.Add(new MarkdownBlock
                                {
                                    Text = currentWordText.ToString().Trim(),
                                    IsHeading = false,
                                    ActiveHeaders = activeHeaders
                                });
                                currentWordText.Clear();
                            }
                        }
                        currentWordText.Append(currentWordText.Length == 0 ? word : $" {word}");
                    }

                    if (currentWordText.Length > 0)
                    {
                        currentSubText.Append(currentWordText.ToString());
                    }
                }
                else
                {
                    currentSubText.Append(sentence);
                }
            }
            else
            {
                currentSubText.Append(currentSubText.Length == 0 ? sentence : $" {sentence}");
            }
        }

        if (currentSubText.Length > 0)
        {
            subBlocks.Add(new MarkdownBlock
            {
                Text = currentSubText.ToString().Trim(),
                IsHeading = false,
                ActiveHeaders = activeHeaders
            });
        }

        return subBlocks;
    }

    public static string GetSentenceOverlap(List<MarkdownBlock> blocks, int overlapTokensLimit)
    {
        if (overlapTokensLimit <= 0 || blocks.Count == 0) return string.Empty;

        // Chỉ lấy text từ các block không phải tiêu đề
        var textBlocks = blocks.Where(b => !b.IsHeading).Select(b => b.Text).ToList();
        if (textBlocks.Count == 0) return string.Empty;

        var combinedText = string.Join(" ", textBlocks);
        var sentences = Regex.Split(combinedText, @"(?<=[.!?])\s+");
        
        var overlapSentences = new List<string>();
        int tokens = 0;

        for (int i = sentences.Length - 1; i >= 0; i--)
        {
            var s = sentences[i].Trim();
            if (string.IsNullOrWhiteSpace(s)) continue;

            int sTokens = GetTokenCount(s);
            if (tokens + sTokens <= overlapTokensLimit)
            {
                overlapSentences.Insert(0, s);
                tokens += sTokens;
            }
            else
            {
                break;
            }
        }

        return string.Join(" ", overlapSentences).Trim();
    }
}
