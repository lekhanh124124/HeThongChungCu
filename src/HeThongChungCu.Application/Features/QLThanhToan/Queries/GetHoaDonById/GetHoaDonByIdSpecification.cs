using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLThanhToan.Queries.GetHoaDonById;

public class GetHoaDonByIdSpecification : BaseSpecification
{
    public GetHoaDonByIdSpecification(int id)
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
    }
}
