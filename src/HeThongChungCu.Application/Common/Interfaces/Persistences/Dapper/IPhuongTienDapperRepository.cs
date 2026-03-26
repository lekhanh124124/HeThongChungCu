using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Application.Features.QLPhuongTien.Queries.LayDSPhuongTienTrongChungCu;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;

public interface IPhuongTienDapperRepository
{
    Task<PagedResult<PhuongTienResponse>> LayDSPhuongTienTrongChungCu(
        LayDSPhuongTienTrongChungCuSpecification spec,
        CancellationToken cancellationToken = default);

    Task<PhuongTienResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
