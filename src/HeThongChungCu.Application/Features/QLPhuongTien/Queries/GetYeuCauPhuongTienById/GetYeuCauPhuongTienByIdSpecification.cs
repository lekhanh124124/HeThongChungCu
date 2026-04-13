using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Queries.GetYeuCauPhuongTienById;

public class GetYeuCauPhuongTienByIdSpecification : BaseSpecification
{
    public GetYeuCauPhuongTienByIdSpecification(int id)
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
    }
}
