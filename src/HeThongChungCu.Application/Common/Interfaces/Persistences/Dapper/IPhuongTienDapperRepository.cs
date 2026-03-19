using HeThongChungCu.Application.Features.PhuongTien.DTOs;
using HeThongChungCu.Application.Features.PhuongTien.Queries.LayDSPhuongTienTrongChungCu;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;

public interface IPhuongTienDapperRepository
{
    Task<PagedResult<PhuongTienResponse>> LayDSPhuongTienTrongChungCu(
        LayDSPhuongTienTrongChungCuSpecification spec,
        CancellationToken cancellationToken = default);

    Task<PhuongTienResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
