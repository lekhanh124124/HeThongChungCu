namespace HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayLichSuCuTru;

public class LayLichSuCuTruSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        nameof(Domain.Entities.ChungCu.QuanHeCuTru.NgayBatDau),
        nameof(Domain.Entities.ChungCu.QuanHeCuTru.NgayKetThuc),
        nameof(Domain.Entities.ChungCu.CanHo.MaCanHo),
        nameof(Domain.Entities.ChungCu.QuanHeCuTru.IsKetThuc),
        nameof(Domain.Entities.ChungCu.QuanHeCuTru.LoaiQuanHeCuTruId)
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
        AddFilter(nameof(Domain.Entities.ChungCu.QuanHeCuTru.IsDeleted), FilterOperator.Equal, false);

        if (canHoId.HasValue)
        {
            AddFilter(nameof(Domain.Entities.ChungCu.QuanHeCuTru.CanHoId), FilterOperator.Equal, canHoId.Value);
        }

        if (userId.HasValue)
        {
            AddFilter(nameof(Domain.Entities.ChungCu.QuanHeCuTru.UserId), FilterOperator.Equal, userId.Value);
        }
    }
}
