using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Features.ToaNha.Queries.GetListToaNha;

public class GetListToaNhaSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id",
        "MaToaNha",
        "TenToaNha"
    };

    public GetListToaNhaSpecification(
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
            AddKeyword("MaToaNha", FilterOperator.Contains, keyword);
            AddKeyword("TenToaNha", FilterOperator.Contains, keyword);
        }
    }
}
