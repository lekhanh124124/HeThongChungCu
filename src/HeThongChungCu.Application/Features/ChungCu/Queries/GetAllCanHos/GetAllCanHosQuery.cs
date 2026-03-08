using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.ChungCu.DTOs;

namespace HeThongChungCu.Application.Features.ChungCu.Queries.GetAllCanHos;

public record GetAllCanHosQuery(
    int? ToaNhaId = null,
    string? Keyword = null,
    string? SortCol = null,
    bool? IsAsc = false,
    int? PageNumber = 1,
    int? PageSize = 20) : IQuery<PagedResult<CanHoDetailResponse>>;
