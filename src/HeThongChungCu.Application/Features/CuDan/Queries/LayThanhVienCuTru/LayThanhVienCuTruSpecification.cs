namespace HeThongChungCu.Application.Features.CuDan.Queries.LayThanhVienCuTru
{
    public class LayThanhVienCuTruSpecification : BaseSpecification
    {
        public LayThanhVienCuTruSpecification(int canHoId) : base(null, null, null, null)
        {
            AddFilter("CanHoId", FilterOperator.Equal, canHoId);
            AddFilter("TrangThaiCuTruId", FilterOperator.Equal, 1);
            AddFilter("IsDeleted", FilterOperator.Equal, false);
        }
    }
}
