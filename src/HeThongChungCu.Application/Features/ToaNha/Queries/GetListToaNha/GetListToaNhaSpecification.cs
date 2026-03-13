namespace HeThongChungCu.Application.Features.ToaNha.Queries.GetListToaNha;

public class GetListToaNhaSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        nameof(Domain.Entities.ChungCu.ToaNha.Id),
        nameof(Domain.Entities.ChungCu.ToaNha.MaToaNha),
        nameof(Domain.Entities.ChungCu.ToaNha.TenToaNha)
    };

    public GetListToaNhaSpecification(
        string? keyword,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize) 
        : base(sortCol, isAsc, pageNumber, pageSize)
    {
        AddFilter(nameof(Domain.Entities.ChungCu.ToaNha.IsDeleted), FilterOperator.Equal, false);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword(nameof(Domain.Entities.ChungCu.ToaNha.MaToaNha), FilterOperator.Contains, keyword);
            AddKeyword(nameof(Domain.Entities.ChungCu.ToaNha.TenToaNha), FilterOperator.Contains, keyword);
        }
    }
}
