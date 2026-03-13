using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.Tang.DTOs;
using HeThongChungCu.Application.Features.Tang.Queries.GetListTang;
using HeThongChungCu.Application.Features.Tang.Queries.GetTangById;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;

public interface ITangDapperRepository
{
    Task<PagedResult<TangDetailResponse>> GetAllAsync(
        GetListTangSpecification spec,
        CancellationToken cancellationToken = default);

    Task<TangResponse?> GetByIdAsync(
        GetTangByIdSpecification spec, 
        CancellationToken cancellationToken = default);
}
