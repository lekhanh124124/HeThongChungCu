using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetHangMucBaoTriById;

public class GetHangMucBaoTriByIdSpecification : BaseSpecification
{
    public GetHangMucBaoTriByIdSpecification(int id)
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
        AddFilter("IsDeleted", FilterOperator.Equal, false);
    }
}
