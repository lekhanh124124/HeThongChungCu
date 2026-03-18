namespace HeThongChungCu.Application.Common.Interfaces.Persistences.EF;

public interface IToaNhaEFRepository
{
    Task<ToaNha?> GetToaNhaById(int id, CancellationToken cancellationToken = default);
    Task<ToaNha?> GetToaNhaByTangId(int tangId, CancellationToken cancellationToken = default);
    Task<bool> MaToaNhaExistsAsync(string maToaNha, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ToaNha>> GetToaNhaByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Tang>> GetTangByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);

    Task AddAsync(ToaNha toaNha, CancellationToken cancellationToken = default);
    void Update(ToaNha toaNha);
    void Remove(object entity);
}
