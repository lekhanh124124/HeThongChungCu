using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.Profile.Queries.LayQuanHeCuTru;

public class LayQuanHeCuTruSpecification : BaseSpecification
{
    public LayQuanHeCuTruSpecification(int userId) 
        : base(null, null, null, null)
    {
        AddFilter("UserId", FilterOperator.Equal, userId);
        AddFilter("IsKetThuc", FilterOperator.Equal, false);
    }
}
