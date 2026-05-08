using System;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLPhanAnh.DTOs;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Queries.GetPhanAnhList;

public record GetPhanAnhListQuery : IQuery<PagedResult<PhanAnhResponse>>
{
    public int? CanHoId { get; init; }
    public int? TrangThaiPhanAnhId { get; init; }
    public int? LoaiPhanAnhId { get; init; }
    public int? NguoiXuLyId { get; init; }
    public string? Keyword { get; init; }
    public DateTimeOffset? NgayTaoTu { get; init; }
    public DateTimeOffset? NgayTaoDen { get; init; }
    
    public string? SortCol { get; init; }
    public bool? IsAsc { get; init; }
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
