using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetPhieuBaoTriById;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetPhieuBaoTriList;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IPhieuBaoTriQueryRepository
{
    Task<PhieuBaoTriDetailResponse?> GetByIdAsync(GetPhieuBaoTriByIdSpecification spec, CancellationToken cancellationToken = default);
    Task<PagedResult<PhieuBaoTriResponse>> GetListAsync(GetPhieuBaoTriListSpecification spec, CancellationToken cancellationToken = default);
}
