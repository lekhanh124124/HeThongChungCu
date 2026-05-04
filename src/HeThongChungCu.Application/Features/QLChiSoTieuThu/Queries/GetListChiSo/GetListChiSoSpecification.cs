using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.GetListChiSo;

public class GetListChiSoSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id", "MaCanHo", "TenCanHo", "TenDichVu", "Thang", "Nam", "NgayGhiNhan", "TrangThaiChiSoId"
    };

    public GetListChiSoSpecification(string? sortCol, bool? isAsc, int? pageIndex, int? pageSize,
        int? thang, int? nam, int? dichVuId, int? trangThaiChiSoId) 
        : base(sortCol, isAsc, pageIndex, pageSize)
    {
        AddFilter("IsDeleted", FilterOperator.Equal, false);

        if (thang.HasValue) AddFilter("Thang", FilterOperator.Equal, thang.Value);
        if (nam.HasValue) AddFilter("Nam", FilterOperator.Equal, nam.Value);
        if (dichVuId.HasValue) AddFilter("DichVuId", FilterOperator.Equal, dichVuId.Value);
        if (trangThaiChiSoId.HasValue) AddFilter("TrangThaiChiSoId", FilterOperator.Equal, trangThaiChiSoId.Value);
    }
}
