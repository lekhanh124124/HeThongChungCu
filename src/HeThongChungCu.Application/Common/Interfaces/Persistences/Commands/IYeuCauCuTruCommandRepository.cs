using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using System.Linq.Expressions;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IYeuCauCuTruCommandRepository
{
    Task<YeuCauCuTru?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<YeuCauCuTru>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<IEnumerable<YeuCauCuTru>> GetByCanHoIdAsync(int canHoId, CancellationToken cancellationToken = default);
    Task<IEnumerable<YeuCauCuTru>> GetByCanHoIdAndStatusesAsync(int canHoId, IEnumerable<TrangThaiYeuCau> statuses, CancellationToken cancellationToken = default);
    Task<IEnumerable<YeuCauCuTru>> GetPendingRequestsAsync(CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<YeuCauCuTru, bool>> expression, CancellationToken cancellationToken = default);

    Task AddAsync(YeuCauCuTru yeuCau, CancellationToken cancellationToken = default);
    void Update(YeuCauCuTru yeuCau);
    void Delete(YeuCauCuTru yeuCau);
    void DeleteRange(IEnumerable<YeuCauCuTru> yeuCaus);
}
