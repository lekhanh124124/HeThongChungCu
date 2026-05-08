using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLPhanAnh.DTOs;
using HeThongChungCu.Application.Features.QLPhanAnh.Queries.GetPhanAnhList;
using HeThongChungCu.Application.Features.QLPhanAnh.Queries.GetPhanAnhById;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IYeuCauPhanAnhQueryRepository
{
    Task<PagedResult<PhanAnhResponse>> GetAllAsync(GetPhanAnhListSpecification spec, CancellationToken cancellationToken = default);
    Task<PhanAnhDetailResponse?> GetByIdAsync(GetPhanAnhByIdSpecification spec, CancellationToken cancellationToken = default);
}
