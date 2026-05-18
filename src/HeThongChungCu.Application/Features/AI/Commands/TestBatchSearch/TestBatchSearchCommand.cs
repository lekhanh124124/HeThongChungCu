using HeThongChungCu.Application.Common.Messaging;
using System.Collections.Generic;

namespace HeThongChungCu.Application.Features.AI.Commands.TestBatchSearch;

public class TestBatchSearchCommand : ICommand<TestBatchSearchResultDto>
{
    public string CollectionName { get; }
    public List<string> Texts { get; }

    public TestBatchSearchCommand(string collectionName, List<string> texts)
    {
        CollectionName = collectionName;
        Texts = texts;
    }
}
