using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Application.Features.CanHo.Queries.GetCanHoById;
using HeThongChungCu.Application.Features.CanHo.Queries.GetListCanHo;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface ICanHoQueryRepository
{
    Task<PagedResult<CanHoDetailResponse>> GetAllAsync(
        GetListCanHoSpecification spec,
        CancellationToken cancellationToken = default);

    Task<CanHoResponse?> GetByIdAsync(GetCanHoByIdSpecification spec, CancellationToken cancellationToken = default);
}
