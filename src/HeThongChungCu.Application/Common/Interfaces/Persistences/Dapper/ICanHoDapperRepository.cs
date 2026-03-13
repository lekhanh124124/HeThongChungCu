using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Application.Features.CanHo.Queries.GetCanHoById;
using HeThongChungCu.Application.Features.CanHo.Queries.GetListCanHo;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;

public interface ICanHoDapperRepository
{
    Task<PagedResult<CanHoDetailResponse>> GetAllAsync(
        GetListCanHoSpecification spec,
        CancellationToken cancellationToken = default);

    Task<CanHoResponse?> GetByIdAsync(GetCanHoByIdSpecification spec, CancellationToken cancellationToken = default);
}
