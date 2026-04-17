using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLCuTru.Queries.GetYeuCauCuTruById;

public class GetYeuCauCuTruByIdSpecification : BaseSpecification
{
    public GetYeuCauCuTruByIdSpecification(int id)
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
        AddFilter("IsDeleted", FilterOperator.Equal, false);
        AddFilter("LoaiYeuCauCuDan", FilterOperator.Equal, LoaiYeuCauCuDan.CuTru.Value);
        AddFilter("CanHoIsDeleted", FilterOperator.Equal, false);
        AddFilter("TangIsDeleted", FilterOperator.Equal, false);
        AddFilter("ToaNhaIsDeleted", FilterOperator.Equal, false);
        AddFilter("TaiKhoanIsActive", FilterOperator.Equal, true);
        AddFilter("TaiKhoanIsDeleted", FilterOperator.Equal, false);
        AddFilter("TaiLieuIsDeleted", FilterOperator.Equal, false);
        AddFilter("LoaiTaiLieuYeuCau", FilterOperator.Equal, LoaiTaiLieu.YeuCauCuTru.Value);
        AddFilter("TepIsDeleted", FilterOperator.Equal, false);
        AddFilter("LoaiTepYeuCau", FilterOperator.Equal, LoaiTepTaiLieu.YeuCauCuTru.Value);
    }
}
