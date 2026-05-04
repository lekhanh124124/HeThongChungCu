using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IDotThanhToanCommandRepository
{
    Task AddAsync(DotThanhToan dot, CancellationToken cancellationToken = default);
    Task<DotThanhToan?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<DotThanhToan>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<DotThanhToan?> GetLatestOpenByKyAsync(KyThanhToan ky, CancellationToken cancellationToken = default);
    Task<bool> ExistsByKyThanhToanExcludeIdAsync(KyThanhToan ky, int excludeId, CancellationToken cancellationToken = default);
    void Delete(DotThanhToan dot);
    void DeleteRange(IEnumerable<DotThanhToan> dots);
}
