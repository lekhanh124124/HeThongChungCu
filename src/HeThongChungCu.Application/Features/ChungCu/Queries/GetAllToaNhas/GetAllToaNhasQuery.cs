using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.ChungCu.DTOs;

namespace HeThongChungCu.Application.Features.ChungCu.Queries.GetAllToaNhas;

public record GetAllToaNhasQuery(
    string? Keyword = null,
    string? SortCol = null,
    bool? IsAsc = false,
    int? PageNumber = 1,
    int? PageSize = 20) : IQuery<PagedResult<ToaNhaDetailResponse>>;