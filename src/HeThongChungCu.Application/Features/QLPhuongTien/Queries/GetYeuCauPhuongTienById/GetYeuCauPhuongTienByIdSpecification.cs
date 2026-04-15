using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Queries.GetYeuCauPhuongTienById;

public class GetYeuCauPhuongTienByIdSpecification : BaseSpecification
{
    public GetYeuCauPhuongTienByIdSpecification(int id)
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
        AddFilter("IsDeleted", FilterOperator.Equal, false);
        AddFilter("LoaiYeuCauCuDan", FilterOperator.Equal, "YeuCauPhuongTien");
        AddFilter("CanHoIsDeleted", FilterOperator.Equal, false);
        AddFilter("TangIsDeleted", FilterOperator.Equal, false);
        AddFilter("ToaNhaIsDeleted", FilterOperator.Equal, false);
        AddFilter("TaiKhoanIsActive", FilterOperator.Equal, true);
        AddFilter("TaiKhoanIsDeleted", FilterOperator.Equal, false);

        // Related files filters
        AddFilter("LoaiTepYeuCauPhuongTien", FilterOperator.Equal, "TepYeuCauPhuongTien");
        AddFilter("TepIsDeleted", FilterOperator.Equal, false);
    }
}
