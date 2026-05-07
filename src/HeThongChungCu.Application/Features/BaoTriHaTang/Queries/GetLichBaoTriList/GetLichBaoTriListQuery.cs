using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetLichBaoTriList;

public record GetLichBaoTriListQuery(
    int? ThietBiId,
    int? HangMucId,
    string? SortCol,
    bool? IsAsc,
    int? PageNumber = 1,
    int? PageSize = 10) : IQuery<PagedResult<LichBaoTriResponse>>;
