using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Features.Catalog.Queries.LayCauTrucChungCu;

public class LayCauTrucChungCuSpecification : BaseSpecification
{
    public LayCauTrucChungCuSpecification(string? keyword)
        : base(null, null, null, null)
    {
        AddFilter("LoaiTangId", FilterOperator.NotEqual, LoaiTang.TangHam.Value);
        AddFilter("IsDeleted", FilterOperator.Equal, false);
        AddFilter("TangIsDeleted", FilterOperator.Equal, false);
        AddFilter("CanHoIsDeleted", FilterOperator.Equal, false);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword("MaToaNha", FilterOperator.Contains, keyword);
            AddKeyword("TenToaNha", FilterOperator.Contains, keyword);
            AddKeyword("MaTang", FilterOperator.Contains, keyword);
            AddKeyword("TenTang", FilterOperator.Contains, keyword);
            AddKeyword("MaCanHo", FilterOperator.Contains, keyword);
            AddKeyword("TenCanHo", FilterOperator.Contains, keyword);
        }
    }
}
