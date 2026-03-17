namespace HeThongChungCu.Application.Features.CuDan.Queries.LayQuanHeCuTru;

public class LayQuanHeCuTruSpecification : BaseSpecification
{
    public LayQuanHeCuTruSpecification(int userId) 
        : base(null, null, null, null)
    {
        AddFilter("UserId", FilterOperator.Equal, userId);
        AddFilter("IsKetThuc", FilterOperator.Equal, false);
        AddFilter("IsDeleted", FilterOperator.Equal, false);
    }
}
