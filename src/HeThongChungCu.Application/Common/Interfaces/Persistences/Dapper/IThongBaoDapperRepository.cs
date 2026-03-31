using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.ThongBao.Queries.LayDSThongBao;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;

public interface IThongBaoDapperRepository
{
    Task<PagedResult<ThongBaoResponse>> GetDSThongBaoAsync(int userId, int pageNumber, int pageSize, bool? onlyUnread, CancellationToken cancellationToken = default);
}
