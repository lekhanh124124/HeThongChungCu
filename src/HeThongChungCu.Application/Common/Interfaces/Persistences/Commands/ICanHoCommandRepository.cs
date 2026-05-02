namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface ICanHoCommandRepository
{
    Task<CanHo?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> MaCanHoExistsAsync(string maCanHo, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CanHo>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<List<CanHo>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    Task AddAsync(CanHo canHo, CancellationToken cancellationToken = default);
    void Update(CanHo canHo);
    void Remove(CanHo canHo);
}
