using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;
using HeThongChungCu.Application.Features.YeuCauThiCong.Queries.GetListYeuCauThiCong;
using HeThongChungCu.Application.Features.YeuCauThiCong.Queries.GetYeuCauThiCongById;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IYeuCauThiCongQueryRepository
{
    Task<PagedResult<YeuCauThiCongResponse>> GetAllAsync(GetListYeuCauThiCongSpecification spec, CancellationToken cancellationToken = default);
    Task<YeuCauThiCongDetailResponse?> GetByIdAsync(GetYeuCauThiCongByIdSpecification spec, CancellationToken cancellationToken = default);
}
