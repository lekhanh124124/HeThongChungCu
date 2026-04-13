using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Queries.GetPhuongTienById;

public class GetPhuongTienByIdSpecification : BaseSpecification
{
    public GetPhuongTienByIdSpecification(int id)
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
    }
}
