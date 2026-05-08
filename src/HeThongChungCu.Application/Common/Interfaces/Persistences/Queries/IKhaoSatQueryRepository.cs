using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLKhaoSat.DTOs;
using HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKhaoSatList;
using HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKhaoSatById;
using HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKetQuaKhaoSat;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IKhaoSatQueryRepository
{
    Task<PagedResult<KhaoSatResponse>> GetAllAsync(GetKhaoSatListSpecification spec, CancellationToken cancellationToken = default);
    Task<KhaoSatDetailResponse?> GetByIdAsync(GetKhaoSatByIdSpecification spec, CancellationToken cancellationToken = default);
    Task<KetQuaKhaoSatResponse?> GetKetQuaKhaoSatAsync(GetKetQuaKhaoSatSpecification spec, CancellationToken cancellationToken = default);
}
