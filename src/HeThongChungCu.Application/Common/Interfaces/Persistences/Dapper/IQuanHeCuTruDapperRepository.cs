using HeThongChungCu.Application.Features.Profile.DTOs;
using HeThongChungCu.Application.Features.Profile.Queries.LayQuanHeCuTru;
using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;
using HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayCuDanByCanHoId;
using HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayLichSuCuTru;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;

public interface IQuanHeCuTruDapperRepository
{
    Task<IReadOnlyList<CuDanResponse>> GetCuDanByCanHoIdAsync(
        LayCuDanByCanHoIdSpecification spec,
        CancellationToken cancellationToken = default);

    Task<PagedResult<LichSuCuTruResponse>> GetLichSuAsync(
        LayLichSuCuTruSpecification spec,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LayQuanHeCuTruResponse>> GetActiveByUserIdAsync(
        LayQuanHeCuTruSpecification spec,
        CancellationToken cancellationToken = default);
}
