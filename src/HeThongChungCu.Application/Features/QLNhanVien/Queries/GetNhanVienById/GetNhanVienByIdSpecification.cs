using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLNhanVien.Queries.GetNhanVienById;

public class GetNhanVienByIdSpecification : BaseSpecification
{
    public GetNhanVienByIdSpecification(int id)
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
        AddFilter("IsDeleted", FilterOperator.Equal, false);
    }
}
