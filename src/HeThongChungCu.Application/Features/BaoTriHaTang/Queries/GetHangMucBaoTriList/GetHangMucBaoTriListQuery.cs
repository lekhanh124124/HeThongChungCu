using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetHangMucBaoTriList;

public record GetHangMucBaoTriListQuery(
    string? Keyword,
    string? SortCol,
    bool? IsAsc,
    int? PageNumber = 1,
    int? PageSize = 10) : IQuery<PagedResult<HangMucBaoTriResponse>>;
