using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetQuyThuChiById;

public class GetQuyThuChiByIdSpecification : BaseSpecification
{
    public GetQuyThuChiByIdSpecification(int id) 
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
        AddFilter("IsDeleted", FilterOperator.Equal, false);
    }
}
