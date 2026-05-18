using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.AI.Commands.SyncKnowledgeBase;

public class SyncKnowledgeBaseCommand : ICommand<SyncResultDto>
{
    public bool ForceRebuild { get; set; } = false;
    public int? MaxFilesToSync { get; set; }

    public SyncKnowledgeBaseCommand()
    {
    }

    public SyncKnowledgeBaseCommand(bool forceRebuild = false, int? maxFilesToSync = null)
    {
        ForceRebuild = forceRebuild;
        MaxFilesToSync = maxFilesToSync;
    }
}
