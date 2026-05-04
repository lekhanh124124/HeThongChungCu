using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.GetChiSoById;

public class GetChiSoByIdSpecification : BaseSpecification
{
    public GetChiSoByIdSpecification(int id) 
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
        AddFilter("IsDeleted", FilterOperator.Equal, false);
    }
}
