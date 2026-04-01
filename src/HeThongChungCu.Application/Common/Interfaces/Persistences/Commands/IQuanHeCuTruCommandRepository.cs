namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IQuanHeCuTruCommandRepository
{
    Task<QuanHeCuTru?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<QuanHeCuTru?> GetCuTruByCanHoIdAsync(int canHoId, CancellationToken cancellationToken = default);
    Task AddAsync(QuanHeCuTru quanHeCuTru, CancellationToken cancellationToken = default);
    void Update(QuanHeCuTru quanHeCuTru);
    void Remove(QuanHeCuTru quanHeCuTru);
    Task<IEnumerable<QuanHeCuTru>> GetByCanHoIdAsync(int canHoId, CancellationToken cancellationToken = default);
    Task<QuanHeCuTru?> GetByUserAndCanHoAsync(int userId, int canHoId, CancellationToken cancellationToken = default);
}
