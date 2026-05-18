using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.AI.Commands.TestLLM;

public class TestLLMCommand : ICommand<TestLLMResultDto>
{
    public string Prompt { get; }

    public TestLLMCommand(string prompt)
    {
        Prompt = prompt;
    }
}
