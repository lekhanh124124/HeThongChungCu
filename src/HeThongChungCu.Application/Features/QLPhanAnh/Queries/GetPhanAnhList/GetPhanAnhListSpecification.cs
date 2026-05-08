using System;
using System.Collections.Generic;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Queries.GetPhanAnhList;

public class GetPhanAnhListSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id", "CreatedAt", "TieuDe"
    };

    public GetPhanAnhListSpecification(
        int? canHoId,
        int? trangThaiPhanAnhId,
        int? loaiPhanAnhId,
        int? nguoiXuLyId,
        string? keyword,
        DateTimeOffset? ngayTaoTu,
        DateTimeOffset? ngayTaoDen,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize) : base(sortCol, isAsc, pageNumber, pageSize)
    {
        AddFilter("YeuCauLoai", FilterOperator.Equal, LoaiYeuCauCuDan.PhanAnh.Value);
        AddFilter("YeuCauIsDeleted", FilterOperator.Equal, false);
        AddFilter("CanHoIsDeleted", FilterOperator.Equal, false);

        if (canHoId.HasValue)
            AddFilter("CanHoId", FilterOperator.Equal, canHoId.Value);

        if (trangThaiPhanAnhId.HasValue)
            AddFilter("TrangThaiPhanAnhId", FilterOperator.Equal, trangThaiPhanAnhId.Value);

        if (loaiPhanAnhId.HasValue)
            AddFilter("LoaiPhanAnhId", FilterOperator.Equal, loaiPhanAnhId.Value);

        if (nguoiXuLyId.HasValue)
            AddFilter("NguoiXuLyId", FilterOperator.Equal, nguoiXuLyId.Value);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword("TieuDe", FilterOperator.Contains, keyword);
            AddKeyword("NoiDung", FilterOperator.Contains, keyword);
        }

        if (ngayTaoTu.HasValue)
            AddFilter("CreatedAt", FilterOperator.GreaterThanOrEqual, ngayTaoTu.Value);

        if (ngayTaoDen.HasValue)
            AddFilter("CreatedAt", FilterOperator.LessThanOrEqual, ngayTaoDen.Value);
    }
}
