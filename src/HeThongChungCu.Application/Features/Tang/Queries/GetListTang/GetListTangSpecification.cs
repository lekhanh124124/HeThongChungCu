namespace HeThongChungCu.Application.Features.Tang.Queries.GetListTang;

public class GetListTangSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        nameof(Domain.Entities.ChungCu.Tang.Id),
        nameof(Domain.Entities.ChungCu.Tang.MaTang),
        nameof(Domain.Entities.ChungCu.Tang.TenTang),
        nameof(Domain.Entities.ChungCu.Tang.ToaNhaId),
        nameof(Domain.Entities.ChungCu.Tang.LoaiTangId)
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
        AddFilter(nameof(Domain.Entities.ChungCu.Tang.IsDeleted), FilterOperator.Equal, false);

        if (toaNhaId.HasValue)
        {
            AddFilter(nameof(Domain.Entities.ChungCu.Tang.ToaNhaId), FilterOperator.Equal, toaNhaId.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword(nameof(Domain.Entities.ChungCu.Tang.MaTang), FilterOperator.Contains, keyword);
            AddKeyword(nameof(Domain.Entities.ChungCu.Tang.TenTang), FilterOperator.Contains, keyword);
        }
    }
}
