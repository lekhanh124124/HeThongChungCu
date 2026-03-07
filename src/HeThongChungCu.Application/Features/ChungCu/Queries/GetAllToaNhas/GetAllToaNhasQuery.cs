using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.ChungCu.DTOs;

namespace HeThongChungCu.Application.Features.ChungCu.Queries.GetAllToaNhas;

public record GetAllToaNhasQuery(
    string? Keyword = null,
    string? SortBy = null,
    bool IsDescending = false,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PagedResult<ToaNhaDetailResponse>>;