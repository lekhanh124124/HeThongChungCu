using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.ToaNha.DTOs;

namespace HeThongChungCu.Application.Features.ToaNha.Queries.GetListToaNha;

public record GetListToaNhaQuery(
    string? Keyword = null,
    string? SortCol = null,
    bool? IsAsc = false,
    int? PageNumber = 1,
    int? PageSize = 20) : IQuery<PagedResult<ToaNhaDetailResponse>>;
