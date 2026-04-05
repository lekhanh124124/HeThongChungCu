using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.ThongBao.Queries.LayDSThongBao;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

public interface IThongBaoQueryRepository
{
    Task<PagedResult<ThongBaoResponse>> GetDSThongBaoAsync(LayDSThongBaoSpecification spec, CancellationToken cancellationToken = default);
}
