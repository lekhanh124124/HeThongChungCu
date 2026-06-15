using HeThongChungCu.Domain.Entities;
using System.Linq.Expressions;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IYeuCauThiCongCommandRepository
{
    Task<YeuCauThiCong?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<YeuCauThiCong?> GetByIdWithFilesAsync(int id, CancellationToken cancellationToken = default);
    Task<YeuCauThiCong?> GetByIdWithPersonnelAsync(int id, CancellationToken cancellationToken = default);
    Task<YeuCauThiCong?> GetByIdWithAllAsync(int id, CancellationToken cancellationToken = default);
    Task<List<YeuCauThiCong>> GetByCanHoIdAndStatusesAsync(int canHoId, IEnumerable<HeThongChungCu.Domain.Enums.TrangThaiYeuCau> statuses, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<YeuCauThiCong, bool>> expression, CancellationToken cancellationToken = default);

    Task AddAsync(YeuCauThiCong yctc, CancellationToken cancellationToken = default);
    void Update(YeuCauThiCong yctc);
    void Delete(YeuCauThiCong yctc);
}
