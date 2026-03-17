using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Features.CanHo.Queries.GetListCanHo;

public class GetListCanHoSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        nameof(global::HeThongChungCu.Domain.Entities.CanHo.Id),
        nameof(global::HeThongChungCu.Domain.Entities.CanHo.MaCanHo),
        nameof(global::HeThongChungCu.Domain.Entities.CanHo.DienTich),
        nameof(global::HeThongChungCu.Domain.Entities.CanHo.SoPhongNgu),
        nameof(global::HeThongChungCu.Domain.Entities.CanHo.SoPhongTam),
        nameof(global::HeThongChungCu.Domain.Entities.CanHo.TinhTrangCanHoId),

        nameof(global::HeThongChungCu.Domain.Entities.CanHo.TangId),
        nameof(global::HeThongChungCu.Domain.Entities.Tang.TenTang),
        nameof(global::HeThongChungCu.Domain.Entities.CanHo.TenCanHo),
        nameof(global::HeThongChungCu.Domain.Entities.CanHo.LoaiCanHoId),
    };

    public GetListCanHoSpecification(
        int? tangId,
        string? keyword,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize) 
        : base(sortCol, isAsc, pageNumber, pageSize)
    {
        AddFilter(nameof(global::HeThongChungCu.Domain.Entities.CanHo.IsDeleted), FilterOperator.Equal, false);

        if (tangId.HasValue)
        {
            AddFilter(nameof(global::HeThongChungCu.Domain.Entities.CanHo.TangId), FilterOperator.Equal, tangId.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword(nameof(global::HeThongChungCu.Domain.Entities.CanHo.MaCanHo), FilterOperator.Contains, keyword);
            AddKeyword(nameof(global::HeThongChungCu.Domain.Entities.CanHo.TenCanHo), FilterOperator.Contains, keyword);
        }
    }
}
