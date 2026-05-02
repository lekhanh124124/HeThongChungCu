using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Queries.GetYeuCauThiCongById;

public class GetYeuCauThiCongByIdSpecification : BaseSpecification
{
    public GetYeuCauThiCongByIdSpecification(int id) : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
        AddFilter("YeuCauLoai", FilterOperator.Equal, LoaiYeuCauCuDan.ThiCong.Value);
        AddFilter("YeuCauIsDeleted", FilterOperator.Equal, false);
        AddFilter("CanHoIsDeleted", FilterOperator.Equal, false);
        AddFilter("NhanSuLoai", FilterOperator.Equal, LoaiNhanSuYeuCau.ThiCong.Value);
        AddFilter("NhanSuIsDeleted", FilterOperator.Equal, false);
        AddFilter("TepLoai", FilterOperator.Equal, LoaiTepTaiLieu.YeuCauThiCong.Value);
        AddFilter("TepIsDeleted", FilterOperator.Equal, false);
    }
}
