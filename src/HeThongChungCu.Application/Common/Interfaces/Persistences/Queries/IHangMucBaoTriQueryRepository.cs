using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetHangMucBaoTriById;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetHangMucBaoTriList;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IHangMucBaoTriQueryRepository
{
    Task<HangMucBaoTriDetailResponse?> GetByIdAsync(GetHangMucBaoTriByIdSpecification spec, CancellationToken cancellationToken = default);
    Task<PagedResult<HangMucBaoTriResponse>> GetListAsync(GetHangMucBaoTriListSpecification spec, CancellationToken cancellationToken = default);
}
