using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.Tang.DTOs;
using HeThongChungCu.Application.Features.Tang.Queries.GetListTang;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;

public interface ITangDapperRepository
{
    Task<PagedResult<TangDetailResponse>> GetAllAsync(
        GetListTangSpecification spec,
        CancellationToken cancellationToken = default);

    Task<TangResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
