namespace HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayCuDanByCanHoId;

public class LayCuDanByCanHoIdSpecification : BaseSpecification
{
    public LayCuDanByCanHoIdSpecification(int canHoId) 
        : base(null, null, null, null)
    {
        AddFilter(nameof(Domain.Entities.ChungCu.QuanHeCuTru.CanHoId), FilterOperator.Equal, canHoId);
        AddFilter(nameof(Domain.Entities.ChungCu.QuanHeCuTru.IsKetThuc), FilterOperator.Equal, false);
        AddFilter(nameof(Domain.Entities.ChungCu.QuanHeCuTru.IsDeleted), FilterOperator.Equal, false);
    }
}
