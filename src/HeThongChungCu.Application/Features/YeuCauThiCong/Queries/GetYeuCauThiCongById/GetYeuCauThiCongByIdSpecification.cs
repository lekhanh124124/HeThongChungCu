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
    }
}
