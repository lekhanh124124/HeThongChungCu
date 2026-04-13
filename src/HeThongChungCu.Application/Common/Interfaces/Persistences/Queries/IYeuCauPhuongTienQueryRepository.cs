using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Application.Features.QLPhuongTien.Queries.LayDSYeuCauPhuongTien;
using HeThongChungCu.Application.Features.QLPhuongTien.Queries.GetYeuCauPhuongTienById;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IYeuCauPhuongTienQueryRepository
{
    Task<PagedResult<DSYeuCauPhuongTienResponse>> GetPagedListAsync(
        LayDSYeuCauPhuongTienQuerySpecification spec,
        CancellationToken cancellationToken = default);

    Task<YeuCauPhuongTienResponse?> GetByIdAsync(GetYeuCauPhuongTienByIdSpecification spec, CancellationToken cancellationToken = default);
    Task<DSYeuCauPhuongTienResponse?> GetListResponseByIdAsync(GetYeuCauPhuongTienByIdSpecification spec, CancellationToken cancellationToken = default);
}
