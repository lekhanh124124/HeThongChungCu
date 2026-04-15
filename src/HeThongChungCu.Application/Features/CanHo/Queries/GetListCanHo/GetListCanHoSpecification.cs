using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Features.CanHo.Queries.GetListCanHo;

public class GetListCanHoSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id",
        "MaCanHo",
        "DienTich",
        "SoPhongNgu",
        "SoPhongTam",
        "TinhTrangCanHoId",

        "TangId",
        "TenTang",
        "TenCanHo",
        "LoaiCanHoId",
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
        AddFilter("IsDeleted", FilterOperator.Equal, false);
        AddFilter("TangIsDeleted", FilterOperator.Equal, false);

        if (tangId.HasValue)
        {
            AddFilter("TangId", FilterOperator.Equal, tangId.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword("MaCanHo", FilterOperator.Contains, keyword);
            AddKeyword("TenCanHo", FilterOperator.Contains, keyword);
        }
    }
}
