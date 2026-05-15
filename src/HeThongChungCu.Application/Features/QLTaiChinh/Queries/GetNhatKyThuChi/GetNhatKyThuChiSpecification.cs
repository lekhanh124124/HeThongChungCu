using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using System;
using System.Collections.Generic;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetNhatKyThuChi;

public class GetNhatKyThuChiSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id", "MaGiaoDich", "NgayGiaoDich", "TongSoTien"
    };

    public GetNhatKyThuChiSpecification(
        int? loaiGiaoDichId,
        int? dichVuId,
        string? nhomThongKe,
        DateTimeOffset? tuNgay,
        DateTimeOffset? denNgay,
        string? searchKey,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize)
        : base(sortCol, isAsc, pageNumber, pageSize)
    {
        if (loaiGiaoDichId.HasValue)
        {
            AddFilter("LoaiGiaoDichId", FilterOperator.Equal, loaiGiaoDichId.Value);
        }

        if (tuNgay.HasValue)
        {
            AddFilter("NgayGiaoDich", FilterOperator.GreaterThanOrEqual, tuNgay.Value);
        }

        if (denNgay.HasValue)
        {
            AddFilter("NgayGiaoDich", FilterOperator.LessThanOrEqual, denNgay.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchKey))
        {
            AddKeyword("MaGiaoDich", FilterOperator.Contains, searchKey);
            AddKeyword("NguoiGiaoDich", FilterOperator.Contains, searchKey);
            AddKeyword("ChungTuGoc", FilterOperator.Contains, searchKey);
        }

        if (dichVuId.HasValue)
        {
            AddFilter("CtDichVuId", FilterOperator.Equal, dichVuId.Value);
        }

        if (!string.IsNullOrWhiteSpace(nhomThongKe))
        {
            AddFilter("CtNhomThongKe", FilterOperator.Contains, nhomThongKe);
        }

        AddFilter("IsDeleted", FilterOperator.Equal, false);
    }
}

