using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetBaoCaoCongNoCanHo;

public class GetBaoCaoCongNoCanHoSpecification : BaseSpecification
{
    public int? ToaNhaId { get; }
    public int Thang { get; }
    public int Nam { get; }

    public GetBaoCaoCongNoCanHoSpecification(int? toaNhaId, int thang, int nam)
        : base(null, null, null, null)
    {
        ToaNhaId = toaNhaId;
        Thang = thang;
        Nam = nam;

        if (toaNhaId.HasValue)
        {
            AddFilter("ToaNhaId", FilterOperator.Equal, toaNhaId.Value);
        }
        AddFilter("Thang", FilterOperator.Equal, thang);
        AddFilter("Nam", FilterOperator.Equal, nam);
    }
}
