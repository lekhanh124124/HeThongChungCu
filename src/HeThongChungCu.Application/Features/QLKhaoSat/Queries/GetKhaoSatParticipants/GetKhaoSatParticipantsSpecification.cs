using System;
using System.Collections.Generic;
using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKhaoSatParticipants;

public class GetKhaoSatParticipantsSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id", "ThoiGianBieuQuyet", "MaCanHo"
    };

    public GetKhaoSatParticipantsSpecification(
        int khaoSatId,
        int? pageNumber,
        int? pageSize) : base(null, null, pageNumber, pageSize)
    {
        AddFilter("BieuQuyetIsDeleted", FilterOperator.Equal, false);
        AddFilter("KhaoSatId", FilterOperator.Equal, khaoSatId);
    }
}
