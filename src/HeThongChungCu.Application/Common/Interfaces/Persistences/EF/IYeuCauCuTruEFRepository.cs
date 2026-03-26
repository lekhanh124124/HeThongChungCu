using HeThongChungCu.Domain.Entities;
using System.Linq.Expressions;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.EF;

public interface IYeuCauCuTruEFRepository
{
    Task<YeuCauCuTru?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<YeuCauCuTru>> GetByCanHoIdAsync(int canHoId, CancellationToken cancellationToken = default);
    Task<IEnumerable<YeuCauCuTru>> GetPendingRequestsAsync(CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<YeuCauCuTru, bool>> expression, CancellationToken cancellationToken = default);

    Task AddAsync(YeuCauCuTru yeuCau, CancellationToken cancellationToken = default);
    void Update(YeuCauCuTru yeuCau);
    void Delete(YeuCauCuTru yeuCau);
}
