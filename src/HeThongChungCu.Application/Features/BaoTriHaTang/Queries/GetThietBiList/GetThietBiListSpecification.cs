using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetThietBiList;

public class GetThietBiListSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id",
        "MaThietBi",
        "TenThietBi",
        "LoaiThietBi",
        "TrangThaiThietBiId"
    };

    public GetThietBiListSpecification(
        string? keyword,
        int? trangThaiThietBiId,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize)
        : base(sortCol, isAsc, pageNumber, pageSize)
    {
        AddFilter("IsDeleted", FilterOperator.Equal, false);

        if (trangThaiThietBiId.HasValue)
        {
            AddFilter("TrangThaiThietBiId", FilterOperator.Equal, trangThaiThietBiId.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword("MaThietBi", FilterOperator.Contains, keyword);
            AddKeyword("TenThietBi", FilterOperator.Contains, keyword);
            AddKeyword("LoaiThietBi", FilterOperator.Contains, keyword);
        }
    }
}
