using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Application.Features.QLPhuongTien.Queries.LayDSPhuongTienTrongChungCu;
using HeThongChungCu.Application.Features.QLPhuongTien.Queries.GetPhuongTienById;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IPhuongTienQueryRepository
{
    Task<PagedResult<PhuongTienResponse>> LayDSPhuongTienTrongChungCu(
        LayDSPhuongTienTrongChungCuSpecification spec,
        CancellationToken cancellationToken = default);

    Task<PhuongTienResponse?> GetByIdAsync(GetPhuongTienByIdSpecification spec, CancellationToken cancellationToken = default);
}
