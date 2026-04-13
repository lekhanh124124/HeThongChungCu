using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Features.Tang.Queries.GetTangById;

public class GetTangByIdSpecification : BaseSpecification
{
    public GetTangByIdSpecification(int id)
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
    }
}
