using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetLichBaoTriList;

public class GetLichBaoTriListSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id",
        "ThietBiId",
        "HangMucBaoTriId",
        "TanSuatBaoTriId",
        "NgayBatDau",
        "NgayBaoTriTiepTheo"
    };

    public GetLichBaoTriListSpecification(
        int? thietBiId,
        int? hangMucBaoTriId,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize)
        : base(sortCol, isAsc, pageNumber, pageSize)
    {
        AddFilter("IsDeleted", FilterOperator.Equal, false);

        if (thietBiId.HasValue)
        {
            AddFilter("ThietBiId", FilterOperator.Equal, thietBiId.Value);
        }

        if (hangMucBaoTriId.HasValue)
        {
            AddFilter("HangMucBaoTriId", FilterOperator.Equal, hangMucBaoTriId.Value);
        }
    }
}
