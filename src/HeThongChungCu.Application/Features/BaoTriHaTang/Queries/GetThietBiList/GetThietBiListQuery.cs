using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetThietBiList;

public record GetThietBiListQuery(
    string? Keyword,
    int? TrangThaiThietBiId,
    int? ToaNhaId,
    string? SortCol,
    bool? IsAsc,
    int? PageNumber = 1,
    int? PageSize = 10) : IQuery<PagedResult<ThietBiResponse>>;
