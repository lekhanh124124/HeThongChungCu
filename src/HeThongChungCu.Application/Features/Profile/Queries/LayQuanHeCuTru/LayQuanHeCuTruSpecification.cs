namespace HeThongChungCu.Application.Features.Profile.Queries.LayQuanHeCuTru;

public class LayQuanHeCuTruSpecification : BaseSpecification
{
    public LayQuanHeCuTruSpecification(int userId) 
        : base(null, null, null, null)
    {
        AddFilter(nameof(Domain.Entities.ChungCu.QuanHeCuTru.UserId), FilterOperator.Equal, userId);
        AddFilter(nameof(Domain.Entities.ChungCu.QuanHeCuTru.IsKetThuc), FilterOperator.Equal, false);
        AddFilter(nameof(Domain.Entities.ChungCu.QuanHeCuTru.IsDeleted), FilterOperator.Equal, false);
    }
}
