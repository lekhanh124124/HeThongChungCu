using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Application.Features.QLPhuongTien.Queries.LayDSYeuCauPhuongTien;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;

public interface IYeuCauPhuongTienDapperRepository
{
    Task<PagedResult<DSYeuCauPhuongTienResponse>> GetPagedListAsync(
        LayDSYeuCauPhuongTienQuerySpecification spec,
        CancellationToken cancellationToken = default);

    Task<YeuCauPhuongTienResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
