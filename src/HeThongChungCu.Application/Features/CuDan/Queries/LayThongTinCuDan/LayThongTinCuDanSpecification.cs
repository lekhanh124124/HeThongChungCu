namespace HeThongChungCu.Application.Features.CuDan.Queries.LayThongTinCuDan;

public class LayThongTinCuDanSpecification : BaseSpecification
{
    public LayThongTinCuDanSpecification(int quanHeCuTruId) 
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, quanHeCuTruId);
        AddFilter("IsDeleted", FilterOperator.Equal, false);
    }
}
