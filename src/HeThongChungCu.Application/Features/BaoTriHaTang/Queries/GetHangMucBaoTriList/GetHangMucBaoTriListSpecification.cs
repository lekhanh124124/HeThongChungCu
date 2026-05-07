using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetHangMucBaoTriList;

public class GetHangMucBaoTriListSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id",
        "MaHangMuc",
        "TenHangMuc"
    };

    public GetHangMucBaoTriListSpecification(
        string? keyword,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize)
        : base(sortCol, isAsc, pageNumber, pageSize)
    {
        AddFilter("IsDeleted", FilterOperator.Equal, false);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword("MaHangMuc", FilterOperator.Contains, keyword);
            AddKeyword("TenHangMuc", FilterOperator.Contains, keyword);
        }
    }
}
