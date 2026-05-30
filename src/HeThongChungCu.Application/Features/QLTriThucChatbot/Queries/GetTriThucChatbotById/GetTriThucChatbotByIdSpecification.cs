using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLTriThucChatbot.Queries.GetTriThucChatbotById;

public class GetTriThucChatbotByIdSpecification : BaseSpecification
{
    public int Id { get; }

    public GetTriThucChatbotByIdSpecification(int id) : base(null, null, null, null)
    {
        Id = id;
        AddFilter("Id", FilterOperator.Equal, id);
        AddFilter("IsDeleted", FilterOperator.Equal, false);
    }

    public override HashSet<string> AllowedSortColumns => new();
}
