namespace HeThongChungCu.Application.Features.Catalog.Queries.LayCauTrucChungCu;

public class LayCauTrucChungCuSpecification : BaseSpecification
{
    public LayCauTrucChungCuSpecification(string? keyword) 
        : base(null, null, null, null)
    {
        AddFilter("ToaNhaIsDeleted", FilterOperator.Equal, false);
        AddFilter("TangIsDeleted", FilterOperator.Equal, false);
        AddFilter("CanHoIsDeleted", FilterOperator.Equal, false);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword(nameof(Domain.Entities.ChungCu.ToaNha.MaToaNha), FilterOperator.Contains, keyword);
            AddKeyword(nameof(Domain.Entities.ChungCu.ToaNha.TenToaNha), FilterOperator.Contains, keyword);
            AddKeyword(nameof(Domain.Entities.ChungCu.Tang.MaTang), FilterOperator.Contains, keyword);
            AddKeyword(nameof(Domain.Entities.ChungCu.Tang.TenTang), FilterOperator.Contains, keyword);
            AddKeyword(nameof(Domain.Entities.ChungCu.CanHo.MaCanHo), FilterOperator.Contains, keyword);
            AddKeyword(nameof(Domain.Entities.ChungCu.CanHo.TenCanHo), FilterOperator.Contains, keyword);
        }
    }
}
