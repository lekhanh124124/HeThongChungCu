using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLDoiTac.Queries.GetDoiTacById;

public class GetDoiTacByIdSpecification : BaseSpecification
{
    public GetDoiTacByIdSpecification(int id)
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
    }
}
