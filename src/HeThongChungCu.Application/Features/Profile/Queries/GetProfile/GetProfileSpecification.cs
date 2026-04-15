namespace HeThongChungCu.Application.Features.Profile.Queries.GetProfile;

public class GetProfileSpecification : BaseSpecification
{
    public GetProfileSpecification(int id) 
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
        AddFilter("IsDeleted", FilterOperator.Equal, false);

        // Account filters
        AddFilter("TaiKhoanIsActive", FilterOperator.Equal, true);
        AddFilter("TaiKhoanIsDeleted", FilterOperator.Equal, false);

        // File filters
        AddFilter("TepIsDeleted", FilterOperator.Equal, false);
        AddFilter("LoaiTepTaiLieu", FilterOperator.Equal, "TepTaiLieu");
    }
}
