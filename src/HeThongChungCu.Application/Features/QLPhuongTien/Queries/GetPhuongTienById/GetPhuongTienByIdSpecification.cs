using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Queries.GetPhuongTienById;

public class GetPhuongTienByIdSpecification : BaseSpecification
{
    public GetPhuongTienByIdSpecification(int id)
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
        AddFilter("IsDeleted", FilterOperator.Equal, false);
        AddFilter("CanHoIsDeleted", FilterOperator.Equal, false);
        AddFilter("TangIsDeleted", FilterOperator.Equal, false);
        AddFilter("ToaNhaIsDeleted", FilterOperator.Equal, false);
        AddFilter("ThePhuongTienIsDeleted", FilterOperator.Equal, false);

        // Related files filters
        AddFilter("LoaiTepPhuongTien", FilterOperator.Equal, "TepPhuongTien");
        AddFilter("TepIsDeleted", FilterOperator.Equal, false);
    }
}
