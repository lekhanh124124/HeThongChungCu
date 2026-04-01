using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using System.Linq.Expressions;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IYeuCauPhuongTienCommandRepository
{
    Task<YeuCauPhuongTien?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<YeuCauPhuongTien>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<IEnumerable<YeuCauPhuongTien>> GetByCanHoIdAsync(int canHoId, CancellationToken cancellationToken = default);
    Task<IEnumerable<YeuCauPhuongTien>> GetByCanHoIdAndStatusesAsync(int canHoId, IEnumerable<TrangThaiYeuCau> statuses, CancellationToken cancellationToken = default);
    Task<IEnumerable<YeuCauPhuongTien>> GetPendingRequestsAsync(CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<YeuCauPhuongTien, bool>> expression, CancellationToken cancellationToken = default);

    Task AddAsync(YeuCauPhuongTien yeuCau, CancellationToken cancellationToken = default);
    void Update(YeuCauPhuongTien yeuCau);
    void Delete(YeuCauPhuongTien yeuCau);
    void DeleteRange(IEnumerable<YeuCauPhuongTien> yeuCaus);
}
