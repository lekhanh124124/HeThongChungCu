using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

namespace HeThongChungCu.Application.Features.QLDoiTac.Queries.GetListDoiTac;

public class GetListDoiTacSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id",
        "TenDoiTac",
        "TenCongTy"
    };

    public GetListDoiTacSpecification(
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
            AddKeyword("TenDoiTac", FilterOperator.Contains, keyword);
            AddKeyword("TenCongTy", FilterOperator.Contains, keyword);
            AddKeyword("Email", FilterOperator.Contains, keyword);
            AddKeyword("SoDienThoai", FilterOperator.Contains, keyword);
        }
    }
}
