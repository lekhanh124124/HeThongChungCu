using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;

namespace HeThongChungCu.Application.Features.QLThanhToan.Queries.GetListDotThanhToan;

public record GetListDotThanhToanQuery : IQuery<PagedResult<DotThanhToanResponse>>
{
    public int? Thang { get; init; }
    public int? Nam { get; init; }
    public int? TrangThaiId { get; init; }
    public string? Keyword { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SortBy { get; init; }
    public bool IsAsc { get; init; } = false;
}
