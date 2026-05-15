using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLTaiChinh.DTOs;
using System;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetNhatKyThuChi;

public record GetNhatKyThuChiQuery : IQuery<PagedResult<QuyThuChiResponse>>
{
    public int? LoaiGiaoDichId { get; init; }
    public int? DichVuId { get; init; }
    public string? NhomThongKe { get; init; }
    public DateTimeOffset? TuNgay { get; init; }
    public DateTimeOffset? DenNgay { get; init; }
    public string? Keyword { get; init; }
    public string? SortCol { get; init; } = "NgayGiaoDich";
    public bool? IsAsc { get; init; } = false;
    public int? PageNumber { get; init; } = 1;
    public int? PageSize { get; init; } = 20;
}
