using HeThongChungCu.Domain.Entities.ChungCu;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.EF;

public interface IToaNhaEFRepository
{
    Task<ToaNha?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> MaToaNhaExistsAsync(string maToaNha, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ToaNha>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);

    Task AddAsync(ToaNha toaNha, CancellationToken cancellationToken = default);
    void Update(ToaNha toaNha);
    void Remove(ToaNha toaNha);
}
