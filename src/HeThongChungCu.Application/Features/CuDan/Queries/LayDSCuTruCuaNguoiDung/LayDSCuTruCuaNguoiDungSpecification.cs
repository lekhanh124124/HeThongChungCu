namespace HeThongChungCu.Application.Features.CuDan.Queries.LayDSCuTruCuaNguoiDung;

public class LayDSCuTruCuaNguoiDungSpecification : BaseSpecification
{
    public LayDSCuTruCuaNguoiDungSpecification(int userId) 
        : base(null, null, null, null)
    {
        AddFilter("NguoiDungId", FilterOperator.Equal, userId);
        AddFilter("TrangThaiCuTruId", FilterOperator.Equal, 1);
        AddFilter("IsDeleted", FilterOperator.Equal, false);
    }
}
