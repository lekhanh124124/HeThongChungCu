using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Queries.GetPhanAnhById;

public class GetPhanAnhByIdSpecification : BaseSpecification
{
    public GetPhanAnhByIdSpecification(int id) : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
        AddFilter("YeuCauLoai", FilterOperator.Equal, LoaiYeuCauCuDan.PhanAnh.Value);
        AddFilter("YeuCauIsDeleted", FilterOperator.Equal, false);
        AddFilter("CanHoIsDeleted", FilterOperator.Equal, false);
        AddFilter("TepLoai", FilterOperator.Equal, LoaiTepTaiLieu.YeuCauPhanAnh.Value);
        AddFilter("TepIsDeleted", FilterOperator.Equal, false);
        AddFilter("TraLoiIsDeleted", FilterOperator.Equal, false);
    }
}
