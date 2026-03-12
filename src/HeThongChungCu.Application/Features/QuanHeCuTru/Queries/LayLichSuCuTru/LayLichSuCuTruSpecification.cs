using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayLichSuCuTru;

public class LayLichSuCuTruSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "NgayBatDau", "NgayKetThuc", "MaCanHo", "IsKetThuc", "LoaiQuanHeCuTruId"
    };

    public LayLichSuCuTruSpecification(
        int? canHoId,
        int? userId,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize) 
        : base(sortCol, isAsc, pageNumber, pageSize)
    {
        if (canHoId.HasValue)
        {
            AddFilter("CanHoId", FilterOperator.Equal, canHoId.Value);
        }

        if (userId.HasValue)
        {
            AddFilter("UserId", FilterOperator.Equal, userId.Value);
        }
    }
}
