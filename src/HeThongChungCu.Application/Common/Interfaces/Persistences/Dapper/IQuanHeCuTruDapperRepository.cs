using HeThongChungCu.Application.Features.CuDan.DTOs;
using HeThongChungCu.Application.Features.CuDan.Queries.LayQuanHeCuTru;
using HeThongChungCu.Application.Features.CuDan.Queries.LayThongTinCuDan;
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

    Task<IReadOnlyList<QuanHeCuTruResponse>> GetActiveByUserIdAsync(
        LayQuanHeCuTruSpecification spec,
        CancellationToken cancellationToken = default);

    Task<LayThongTinCuDanResponse?> GetByIdAsync(
        LayThongTinCuDanSpecification spec,
        CancellationToken cancellationToken = default);
}
