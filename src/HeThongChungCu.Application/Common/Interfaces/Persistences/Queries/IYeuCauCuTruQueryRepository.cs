using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Application.Features.QLCuTru.Queries.LayDSYeuCauCuTru;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IYeuCauCuTruQueryRepository
{
    Task<PagedResult<DSYeuCauCuTruResponse>> GetPagedListAsync(
        LayDSYeuCauCuTruQuerySpecification spec,
        CancellationToken cancellationToken = default);
    Task<YeuCauCuTruResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<DSYeuCauCuTruResponse?> GetListResponseByIdAsync(int id, CancellationToken cancellationToken = default);
}
