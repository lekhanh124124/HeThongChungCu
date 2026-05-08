using System;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLKhaoSat.DTOs;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKhaoSatList;

public record GetKhaoSatListQuery : IQuery<PagedResult<KhaoSatResponse>>
{
    public int? TrangThaiId { get; init; }
    public int? LoaiKhaoSatId { get; init; }
    public string? Keyword { get; init; }
    public DateTimeOffset? NgayTaoTu { get; init; }
    public DateTimeOffset? NgayTaoDen { get; init; }

    public string? SortCol { get; init; }
    public bool? IsAsc { get; init; }
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
