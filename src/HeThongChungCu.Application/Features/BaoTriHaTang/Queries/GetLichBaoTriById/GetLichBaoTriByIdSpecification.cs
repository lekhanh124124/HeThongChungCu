using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetLichBaoTriById;

public class GetLichBaoTriByIdSpecification : BaseSpecification
{
    public GetLichBaoTriByIdSpecification(int id)
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
        AddFilter("IsDeleted", FilterOperator.Equal, false);
    }
}
