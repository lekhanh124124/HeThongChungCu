namespace HeThongChungCu.Application.Common.Interfaces.Persistences.EF;

public interface ITangEFRepository
{
    Task<Tang?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> MaTangExistsAsync(string maTang, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Tang>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);

    Task AddAsync(Tang tang, CancellationToken cancellationToken = default);
    void Update(Tang tang);
    void Remove(Tang tang);
}
