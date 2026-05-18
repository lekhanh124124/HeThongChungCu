using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.AI.Commands.TestEmbedding;

public class TestEmbeddingCommand : ICommand<TestEmbeddingResultDto>
{
    public string Text { get; }

    public TestEmbeddingCommand(string text)
    {
        Text = text;
    }
}
