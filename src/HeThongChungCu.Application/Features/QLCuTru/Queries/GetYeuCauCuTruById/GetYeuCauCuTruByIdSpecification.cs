using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLCuTru.Queries.GetYeuCauCuTruById;

public class GetYeuCauCuTruByIdSpecification : BaseSpecification
{
    public GetYeuCauCuTruByIdSpecification(int id)
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
    }
}
