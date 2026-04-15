using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLDichVu.Queries.GetBangGiaById;

public class GetBangGiaByIdSpecification : BaseSpecification
{
    public GetBangGiaByIdSpecification(int id)
        : base(null, null, null, null)
    {
        AddFilter("IsDeleted", FilterOperator.Equal, false);
        AddFilter("Id", FilterOperator.Equal, id);

        // Lọc Khung giờ đi kèm bảng giá
        AddFilter("KhungGioIsActive", FilterOperator.Equal, true);
        AddFilter("KhungGioIsDeleted", FilterOperator.Equal, false);
    }
}
