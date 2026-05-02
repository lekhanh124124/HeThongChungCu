using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Queries.GetListYeuCauSuaChua;

public class GetListYeuCauSuaChuaSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id",
        "CreatedAt",
        "ChiPhiDuKien"
    };

    public GetListYeuCauSuaChuaSpecification(
        int? pageNumber,
        int? pageSize,
        string? sortCol,
        bool? isAsc,
        int? canHoId,
        int? trangThaiYeuCauId,
        int? trangThaiSuaChuaId,
        int? loaiSuCoId,
        DateTimeOffset? ngayTaoTu,
        DateTimeOffset? ngayTaoDen,
        string? maCanHo,
        string? tenNguoiGui)
        : base(sortCol, isAsc, pageNumber, pageSize)
    {
        AddFilter("YeuCauLoai", FilterOperator.Equal, LoaiYeuCauCuDan.SuaChua.Value);
        AddFilter("YeuCauIsDeleted", FilterOperator.Equal, false);
        AddFilter("CanHoIsDeleted", FilterOperator.Equal, false);

        if (canHoId.HasValue)
        {
            AddFilter("CanHoId", FilterOperator.Equal, canHoId.Value);
        }

        if (trangThaiYeuCauId.HasValue)
        {
            AddFilter("TrangThaiYeuCauId", FilterOperator.Equal, trangThaiYeuCauId.Value);
        }

        if (trangThaiSuaChuaId.HasValue)
        {
            AddFilter("TrangThaiSuaChuaId", FilterOperator.Equal, trangThaiSuaChuaId.Value);
        }

        if (loaiSuCoId.HasValue)
        {
            AddFilter("LoaiSuCoId", FilterOperator.Equal, loaiSuCoId.Value);
        }

        if (ngayTaoTu.HasValue)
        {
            AddFilter("CreatedAt", FilterOperator.GreaterThanOrEqual, ngayTaoTu.Value);
        }

        if (ngayTaoDen.HasValue)
        {
            AddFilter("CreatedAt", FilterOperator.LessThanOrEqual, ngayTaoDen.Value);
        }

        if (!string.IsNullOrWhiteSpace(maCanHo))
        {
            AddFilter("MaCanHo", FilterOperator.Contains, maCanHo);
        }

        if (!string.IsNullOrWhiteSpace(tenNguoiGui))
        {
            AddKeyword("TenNguoiGui", FilterOperator.Contains, tenNguoiGui);
        }
    }

}
