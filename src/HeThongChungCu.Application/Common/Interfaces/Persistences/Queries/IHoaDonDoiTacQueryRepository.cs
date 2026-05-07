using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDoiTac.DTOs;
using HeThongChungCu.Application.Features.QLDoiTac.Queries.GetListHoaDonDoiTac;
using HeThongChungCu.Application.Features.QLDoiTac.Queries.GetHoaDonDoiTacById;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IHoaDonDoiTacQueryRepository
{
    Task<PagedResult<HoaDonDoiTacResponse>> GetListAsync(
        GetListHoaDonDoiTacSpecification spec,
        CancellationToken cancellationToken = default);

    Task<HoaDonDoiTacDetailResponse?> GetByIdAsync(
        GetHoaDonDoiTacByIdSpecification spec,
        CancellationToken cancellationToken = default);
}
