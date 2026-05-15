using System;
using System.Collections.Generic;
using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetResidentSurveyHistory;

public class GetResidentSurveyHistorySpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id", "NgayThamGia"
    };

    public GetResidentSurveyHistorySpecification(int? canHoId, int? khaoSatId) : base(null, null, null, null)
    {
        AddFilter("BieuQuyetIsDeleted", FilterOperator.Equal, false);

        if (canHoId.HasValue)
            AddFilter("CanHoId", FilterOperator.Equal, canHoId.Value);

        if (khaoSatId.HasValue)
            AddFilter("KhaoSatId", FilterOperator.Equal, khaoSatId.Value);
    }
}
