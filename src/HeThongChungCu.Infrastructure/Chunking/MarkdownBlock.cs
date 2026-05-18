using System.Collections.Generic;

namespace HeThongChungCu.Infrastructure.Chunking;

internal sealed class MarkdownBlock
{
    public string Text { get; set; } = string.Empty;
    public bool IsHeading { get; set; }
    public int HeadingLevel { get; set; }
    public List<string> ActiveHeaders { get; set; } = new();
}
