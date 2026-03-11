using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.Tang.DTOs;

namespace HeThongChungCu.Application.Features.Tang.Queries.GetListTang;

public record GetListTangQuery(
    int? ToaNhaId = null,
    string? Keyword = null,
    string? SortCol = null,
    bool? IsAsc = false,
    int? PageNumber = 1,
    int? PageSize = 20) : IQuery<PagedResult<TangDetailResponse>>;
