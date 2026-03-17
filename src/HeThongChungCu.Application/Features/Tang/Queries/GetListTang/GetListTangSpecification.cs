using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Features.Tang.Queries.GetListTang;

public class GetListTangSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id",
        "MaTang",
        "TenTang",
        "ToaNhaId",
        "LoaiTangId"
    };

    public GetListTangSpecification(
        int? toaNhaId,
        string? keyword,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize) 
        : base(sortCol, isAsc, pageNumber, pageSize)
    {
        AddFilter("IsDeleted", FilterOperator.Equal, false);

        if (toaNhaId.HasValue)
        {
            AddFilter("ToaNhaId", FilterOperator.Equal, toaNhaId.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword("MaTang", FilterOperator.Contains, keyword);
            AddKeyword("TenTang", FilterOperator.Contains, keyword);
        }
    }
}
