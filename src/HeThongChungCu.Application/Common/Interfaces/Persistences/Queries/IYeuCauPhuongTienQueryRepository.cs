using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Application.Features.QLPhuongTien.Queries.LayDSYeuCauPhuongTien;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IYeuCauPhuongTienQueryRepository
{
    Task<PagedResult<DSYeuCauPhuongTienResponse>> GetPagedListAsync(
        LayDSYeuCauPhuongTienQuerySpecification spec,
        CancellationToken cancellationToken = default);

    Task<YeuCauPhuongTienResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<DSYeuCauPhuongTienResponse?> GetListResponseByIdAsync(int id, CancellationToken cancellationToken = default);
}
