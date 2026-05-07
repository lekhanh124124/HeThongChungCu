using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetLichBaoTriById;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetLichBaoTriList;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface ILichBaoTriQueryRepository
{
    Task<LichBaoTriDetailResponse?> GetByIdAsync(GetLichBaoTriByIdSpecification spec, CancellationToken cancellationToken = default);
    Task<PagedResult<LichBaoTriResponse>> GetListAsync(GetLichBaoTriListSpecification spec, CancellationToken cancellationToken = default);
}
