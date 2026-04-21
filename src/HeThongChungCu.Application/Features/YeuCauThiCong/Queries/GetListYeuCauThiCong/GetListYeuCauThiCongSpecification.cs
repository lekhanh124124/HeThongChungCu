using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Queries.GetListYeuCauThiCong;

public class GetListYeuCauThiCongSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id", "CreatedAt", "DuKienBatDau", "DuKienKetThuc"
    };

    public GetListYeuCauThiCongSpecification(
        int? canHoId,
        int? trangThaiId,
        int? trangThaiThiCongId,
        string? keyword,
        DateTimeOffset? ngayTaoTu,
        DateTimeOffset? ngayTaoDen,
        DateTimeOffset? batDauTu,
        DateTimeOffset? batDauDen,
        DateTimeOffset? ketThucTu,
        DateTimeOffset? ketThucDen,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize) : base(sortCol, isAsc, pageNumber, pageSize)
    {
        AddFilter("YeuCauLoai", FilterOperator.Equal, LoaiYeuCauCuDan.ThiCong.Value);
        AddFilter("YeuCauIsDeleted", FilterOperator.Equal, false);

        if (canHoId.HasValue)
            AddFilter("CanHoId", FilterOperator.Equal, canHoId.Value);

        if (trangThaiId.HasValue)
            AddFilter("TrangThaiYeuCauId", FilterOperator.Equal, trangThaiId.Value);

        if (trangThaiThiCongId.HasValue)
            AddFilter("TrangThaiThiCongId", FilterOperator.Equal, trangThaiThiCongId.Value);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword("HangMucThiCong", FilterOperator.Contains, keyword);
            AddKeyword("TenDonViThiCong", FilterOperator.Contains, keyword);
        }

        if (ngayTaoTu.HasValue)
            AddFilter("CreatedAt", FilterOperator.GreaterThanOrEqual, ngayTaoTu.Value);

        if (ngayTaoDen.HasValue)
            AddFilter("CreatedAt", FilterOperator.LessThanOrEqual, ngayTaoDen.Value);

        if (batDauTu.HasValue)
            AddFilter("DuKienBatDau", FilterOperator.GreaterThanOrEqual, batDauTu.Value);

        if (batDauDen.HasValue)
            AddFilter("DuKienBatDau", FilterOperator.LessThanOrEqual, batDauDen.Value);

        if (ketThucTu.HasValue)
            AddFilter("DuKienKetThuc", FilterOperator.GreaterThanOrEqual, ketThucTu.Value);

        if (ketThucDen.HasValue)
            AddFilter("DuKienKetThuc", FilterOperator.LessThanOrEqual, ketThucDen.Value);
    }
}
