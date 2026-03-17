using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.EF;

public interface IQuanHeCuTruEFRepository
{
    Task<QuanHeCuTru?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(QuanHeCuTru quanHeCuTru, CancellationToken cancellationToken = default);
    void Update(QuanHeCuTru quanHeCuTru);
    void Remove(QuanHeCuTru quanHeCuTru);
    Task<IEnumerable<QuanHeCuTru>> GetByCanHoIdAsync(int canHoId, CancellationToken cancellationToken = default);
}
