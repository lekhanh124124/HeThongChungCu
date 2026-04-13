using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLDichVu.Queries.GetKhungGioDichVuById;

public class GetKhungGioDichVuByIdSpecification : BaseSpecification
{
    public GetKhungGioDichVuByIdSpecification(int id)
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
    }
}
