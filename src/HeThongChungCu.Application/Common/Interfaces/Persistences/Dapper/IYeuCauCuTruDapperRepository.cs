using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Application.Features.QLCuTru.Queries.LayDSYeuCauCuTru;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;

public interface IYeuCauCuTruDapperRepository
{
    Task<PagedResult<YeuCauCuTruResponse>> GetPagedListAsync(
        LayDSYeuCauCuTruQuerySpecification spec,
        CancellationToken cancellationToken = default);
}
