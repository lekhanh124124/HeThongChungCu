using HeThongChungCu.Domain.Entities.ChungCu;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.EF;

public interface ICanHoEFRepository
{
    Task<CanHo?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CanHo?> GetByIdWithQuanHeAsync(int id, CancellationToken cancellationToken = default);
    Task<CanHo?> GetByIdWithQuanHeForRecordAsync(int quanHeCuTruId, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> MaCanHoExistsAsync(string maCanHo, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CanHo>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);

    Task AddAsync(CanHo canHo, CancellationToken cancellationToken = default);
    void Update(CanHo canHo);
    void Remove(CanHo canHo);
}
