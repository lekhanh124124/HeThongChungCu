namespace HeThongChungCu.Application.Features.CuDan.Queries.LayDSCuTruCuaNguoiDung;

public class LayDSCuTruCuaNguoiDungSpecification : BaseSpecification
{
    public LayDSCuTruCuaNguoiDungSpecification(int userId) 
        : base(null, null, null, null)
    {
        AddFilter("UserId", FilterOperator.Equal, userId);
        AddFilter("IsKetThuc", FilterOperator.Equal, false);
        AddFilter("IsDeleted", FilterOperator.Equal, false);
    }
}
