using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetThietBiById;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetThietBiList;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IThietBiQueryRepository
{
    Task<ThietBiDetailResponse?> GetByIdAsync(GetThietBiByIdSpecification spec, CancellationToken cancellationToken = default);
    Task<PagedResult<ThietBiResponse>> GetListAsync(GetThietBiListSpecification spec, CancellationToken cancellationToken = default);
}
