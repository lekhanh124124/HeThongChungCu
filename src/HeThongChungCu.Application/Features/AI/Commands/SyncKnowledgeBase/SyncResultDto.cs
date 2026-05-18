namespace HeThongChungCu.Application.Features.AI.Commands.SyncKnowledgeBase;

public class SyncResultDto
{
    public int TotalFilesProcessed { get; set; }
    public int TotalChunksIngested { get; set; }
    public string CollectionName { get; set; } = string.Empty;
}
