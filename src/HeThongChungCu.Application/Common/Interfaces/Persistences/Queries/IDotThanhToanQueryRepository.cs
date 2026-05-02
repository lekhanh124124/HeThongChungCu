using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetDotThanhToanById;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetListDotThanhToan;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IDotThanhToanQueryRepository
{
    Task<PagedResult<DotThanhToanResponse>> GetListAsync(GetListDotThanhToanSpecification spec, CancellationToken cancellationToken = default);
    Task<DotThanhToanDetailResponse?> GetByIdAsync(GetDotThanhToanByIdSpecification spec, CancellationToken cancellationToken = default);
}
