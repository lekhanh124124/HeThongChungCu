using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Queries.GetYeuCauSuaChuaById;

public class GetYeuCauSuaChuaByIdSpecification : BaseSpecification
{
    public GetYeuCauSuaChuaByIdSpecification(int id)
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
        AddFilter("YeuCauLoai", FilterOperator.Equal, LoaiYeuCauCuDan.SuaChua.Value);
        AddFilter("YeuCauIsDeleted", FilterOperator.Equal, false);
        AddFilter("NhanSuLoai", FilterOperator.Equal, LoaiNhanSuYeuCau.SuaChua.Value);
        AddFilter("NhanSuIsDeleted", FilterOperator.Equal, false);
        AddFilter("TepLoai", FilterOperator.Equal, LoaiTepTaiLieu.YeuCauSuaChua.Value);
        AddFilter("TepIsDeleted", FilterOperator.Equal, false);
    }
}
