using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetHoaDonById;
using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetListHoaDon;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IHoaDonQueryRepository
{
    Task<PagedResult<HoaDonResponse>> GetListAsync(
        GetListHoaDonSpecification spec,
        CancellationToken cancellationToken = default);

    Task<HoaDonDetailResponse?> GetByIdAsync(
        GetHoaDonByIdSpecification spec,
        CancellationToken cancellationToken = default);
}
