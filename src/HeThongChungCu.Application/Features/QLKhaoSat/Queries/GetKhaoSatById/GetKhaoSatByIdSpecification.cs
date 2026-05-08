using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKhaoSatById;

public class GetKhaoSatByIdSpecification : BaseSpecification
{
    public int? CurrentUserId { get; }

    public GetKhaoSatByIdSpecification(int id, int? currentUserId = null) : base(null, null, null, null)
    {
        CurrentUserId = currentUserId;
        AddFilter("Id", FilterOperator.Equal, id);
        AddFilter("KhaoSatIsDeleted", FilterOperator.Equal, false);
        AddFilter("CauHoiIsDeleted", FilterOperator.Equal, false);
        AddFilter("LuaChonIsDeleted", FilterOperator.Equal, false);
    }
}
