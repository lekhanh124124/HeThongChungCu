using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Application.Features.QLCuTru.Queries.LayDSYeuCauCuTru;
using HeThongChungCu.Application.Features.QLCuTru.Queries.GetYeuCauCuTruById;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IYeuCauCuTruQueryRepository
{
    Task<PagedResult<DSYeuCauCuTruResponse>> GetPagedListAsync(
        LayDSYeuCauCuTruQuerySpecification spec,
        CancellationToken cancellationToken = default);
    Task<YeuCauCuTruResponse?> GetByIdAsync(GetYeuCauCuTruByIdSpecification spec, CancellationToken cancellationToken = default);
    Task<DSYeuCauCuTruResponse?> GetListResponseByIdAsync(GetYeuCauCuTruByIdSpecification spec, CancellationToken cancellationToken = default);
}
