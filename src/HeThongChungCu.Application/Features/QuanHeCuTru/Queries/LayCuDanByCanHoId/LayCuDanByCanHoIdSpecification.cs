using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayCuDanByCanHoId;

public class LayCuDanByCanHoIdSpecification : BaseSpecification
{
    public LayCuDanByCanHoIdSpecification(int canHoId) 
        : base(null, null, null, null)
    {
        AddFilter("CanHoId", FilterOperator.Equal, canHoId);
        AddFilter("IsKetThuc", FilterOperator.Equal, false);
    }
}
