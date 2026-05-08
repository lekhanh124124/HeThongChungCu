using System;
using System.Collections.Generic;
using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKhaoSatList;

public class GetKhaoSatListSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id", "CreatedAt", "NgayBatDau", "NgayKetThuc"
    };

    public int? CurrentUserId { get; }

    public GetKhaoSatListSpecification(
        int? trangThaiId,
        int? loaiKhaoSatId,
        string? keyword,
        DateTimeOffset? ngayTaoTu,
        DateTimeOffset? ngayTaoDen,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize,
        int? currentUserId = null) : base(sortCol, isAsc, pageNumber, pageSize)
    {
        CurrentUserId = currentUserId;
        AddFilter("KhaoSatIsDeleted", FilterOperator.Equal, false);

        if (trangThaiId.HasValue)
            AddFilter("TrangThaiId", FilterOperator.Equal, trangThaiId.Value);

        if (loaiKhaoSatId.HasValue)
            AddFilter("LoaiKhaoSatId", FilterOperator.Equal, loaiKhaoSatId.Value);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword("TieuDe", FilterOperator.Contains, keyword);
            AddKeyword("MoTa", FilterOperator.Contains, keyword);
        }

        if (ngayTaoTu.HasValue)
            AddFilter("CreatedAt", FilterOperator.GreaterThanOrEqual, ngayTaoTu.Value);

        if (ngayTaoDen.HasValue)
            AddFilter("CreatedAt", FilterOperator.LessThanOrEqual, ngayTaoDen.Value);
    }
}
