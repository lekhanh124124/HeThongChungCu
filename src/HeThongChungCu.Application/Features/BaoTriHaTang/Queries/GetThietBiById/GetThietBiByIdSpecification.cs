using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetThietBiById;

public class GetThietBiByIdSpecification : BaseSpecification
{
    public GetThietBiByIdSpecification(int id)
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
        AddFilter("IsDeleted", FilterOperator.Equal, false);
    }
}
