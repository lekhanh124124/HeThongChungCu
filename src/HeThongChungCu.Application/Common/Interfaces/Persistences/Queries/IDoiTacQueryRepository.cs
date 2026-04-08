using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDoiTac.DTOs;
using HeThongChungCu.Application.Features.QLDoiTac.Queries.GetDoiTacById;
using HeThongChungCu.Application.Features.QLDoiTac.Queries.GetListDoiTac;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IDoiTacQueryRepository
{
    Task<PagedResult<DoiTacResponse>> GetAllAsync(
        GetListDoiTacSpecification spec,
        CancellationToken cancellationToken = default);

    Task<DoiTacDetailResponse?> GetByIdAsync(
        GetDoiTacByIdSpecification spec,
        CancellationToken cancellationToken = default);
}
