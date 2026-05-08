using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKetQuaKhaoSat;

public class GetKetQuaKhaoSatSpecification : BaseSpecification
{
    public GetKetQuaKhaoSatSpecification(int id) : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
        AddFilter("KhaoSatIsDeleted", FilterOperator.Equal, false);
        AddFilter("CauHoiIsDeleted", FilterOperator.Equal, false);
        AddFilter("LuaChonIsDeleted", FilterOperator.Equal, false);
        AddFilter("BieuQuyetIsDeleted", FilterOperator.Equal, false);
        AddFilter("ChiTietIsDeleted", FilterOperator.Equal, false);
    }
}
