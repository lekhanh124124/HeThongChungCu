using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;

namespace HeThongChungCu.Application.Features.QLDichVu.Queries.GetListBangGia;

public record GetListBangGiaQuery : IQuery<PagedResult<BangGiaResponse>>
{
    public int? DichVuId { get; init; }
    public string? Keyword { get; init; }
    public bool? IsActive { get; init; }
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
    public string? SortBy { get; init; }
    public bool? IsAsc { get; init; }
}
