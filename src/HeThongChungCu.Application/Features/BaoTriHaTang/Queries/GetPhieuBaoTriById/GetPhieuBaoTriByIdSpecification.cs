using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetPhieuBaoTriById;

public class GetPhieuBaoTriByIdSpecification : BaseSpecification
{
    public GetPhieuBaoTriByIdSpecification(int id)
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
        AddFilter("IsDeleted", FilterOperator.Equal, false);
    }
}
