namespace HeThongChungCu.Application.Features.CuDan.Queries.LayThongTinCuDan;

public class LayThongTinCuDanSpecification : BaseSpecification
{
    public LayThongTinCuDanSpecification(int userId, int quanHeCuTruId) 
        : base(null, null, null, null)
    {
        AddFilter("NguoiDungId", FilterOperator.Equal, userId);
        AddFilter("Id", FilterOperator.Equal, quanHeCuTruId);
        AddFilter("IsDeleted", FilterOperator.Equal, false);
    }
}
