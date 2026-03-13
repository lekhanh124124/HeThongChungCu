namespace HeThongChungCu.Application.Features.CanHo.Queries.GetListCanHo;

public class GetListCanHoSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        nameof(Domain.Entities.ChungCu.CanHo.Id),
        nameof(Domain.Entities.ChungCu.CanHo.MaCanHo),
        nameof(Domain.Entities.ChungCu.CanHo.DienTich),
        nameof(Domain.Entities.ChungCu.CanHo.SoPhongNgu),
        nameof(Domain.Entities.ChungCu.CanHo.SoPhongTam),
        nameof(Domain.Entities.ChungCu.CanHo.TinhTrangCanHoId),

        nameof(Domain.Entities.ChungCu.CanHo.TangId),
        nameof(Domain.Entities.ChungCu.Tang.TenTang),
        nameof(Domain.Entities.ChungCu.CanHo.TenCanHo),
        nameof(Domain.Entities.ChungCu.CanHo.LoaiCanHoId),
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
        AddFilter(nameof(Domain.Entities.ChungCu.CanHo.IsDeleted), FilterOperator.Equal, false);

        if (tangId.HasValue)
        {
            AddFilter(nameof(Domain.Entities.ChungCu.CanHo.TangId), FilterOperator.Equal, tangId.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword(nameof(Domain.Entities.ChungCu.CanHo.MaCanHo), FilterOperator.Contains, keyword);
            AddKeyword(nameof(Domain.Entities.ChungCu.CanHo.TenCanHo), FilterOperator.Contains, keyword);
        }
    }
}
