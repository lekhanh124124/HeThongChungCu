using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetBaoCaoCongNoToaNha;

public class GetBaoCaoCongNoToaNhaSpecification : BaseSpecification
{
    public int Thang { get; }
    public int Nam { get; }

    public GetBaoCaoCongNoToaNhaSpecification(int thang, int nam)
        : base(null, null, null, null)
    {
        Thang = thang;
        Nam = nam;

        AddFilter("Thang", FilterOperator.Equal, thang);
        AddFilter("Nam", FilterOperator.Equal, nam);
    }
}
