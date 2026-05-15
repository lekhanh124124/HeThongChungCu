using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using System;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetBaoCaoThuChi;

public class GetBaoCaoThuChiSpecification : BaseSpecification
{
    public DateTimeOffset TuNgay { get; }
    public DateTimeOffset DenNgay { get; }

    public GetBaoCaoThuChiSpecification(DateTimeOffset tuNgay, DateTimeOffset denNgay)
        : base(null, null, null, null)
    {
        TuNgay = tuNgay;
        DenNgay = denNgay;

        AddFilter("NgayGiaoDich", FilterOperator.GreaterThanOrEqual, tuNgay);
        AddFilter("NgayGiaoDich", FilterOperator.LessThanOrEqual, denNgay);
        AddFilter("IsDeleted", FilterOperator.Equal, false);
    }
}
