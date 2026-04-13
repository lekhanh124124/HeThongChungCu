using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDoiTac.DTOs;

namespace HeThongChungCu.Application.Features.QLDoiTac.Queries.GetListDoiTac;

public record GetListDoiTacsQuery(
    string? Keyword,
    string? SortCol,
    bool? IsAsc,
    int? PageNumber,
    int? PageSize) : IQuery<PagedResult<DoiTacResponse>>;
