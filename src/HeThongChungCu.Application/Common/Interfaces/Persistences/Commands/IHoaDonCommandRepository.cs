using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IHoaDonCommandRepository
{
    Task AddAsync(HoaDon hoaDon, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<HoaDon> hoaDons, CancellationToken cancellationToken = default);
    Task<HoaDon?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<HoaDon>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<List<HoaDon>> GetByDotThanhToanAsync(int dotId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByKyThanhToanAsync(int canHoId, KyThanhToan ky, CancellationToken cancellationToken = default);
    Task<HashSet<int>> GetExistingCanHoIdsByKyAsync(KyThanhToan ky, CancellationToken cancellationToken = default);
}
