using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Queries.GetListYeuCauSuaChua;
using HeThongChungCu.Application.Features.YeuCauSuaChua.Queries.GetYeuCauSuaChuaById;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IYeuCauSuaChuaQueryRepository
{
    Task<PagedResult<YeuCauSuaChuaResponse>> GetAllAsync(GetListYeuCauSuaChuaSpecification spec, CancellationToken cancellationToken = default);
    Task<YeuCauSuaChuaDetailResponse?> GetByIdAsync(GetYeuCauSuaChuaByIdSpecification spec, CancellationToken cancellationToken = default);
}
