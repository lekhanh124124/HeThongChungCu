using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IDoiTacCommandRepository
{
    Task<DoiTac?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<DoiTac?> GetByIdWithHopDongsAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DoiTac>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<HopDongDoiTac?> GetHopDongByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(DoiTac doiTac, CancellationToken cancellationToken = default);
    void Update(DoiTac doiTac);
    void Remove(DoiTac doiTac);
    void RemoveRange(IEnumerable<DoiTac> doiTacs);
}
