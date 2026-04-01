using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IDichVuCommandRepository
{
    Task<DichVu?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DichVu>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> MaDichVuExistsAsync(string maDichVu, CancellationToken cancellationToken = default);
    Task AddAsync(DichVu dichVu, CancellationToken cancellationToken = default);
    void Update(DichVu dichVu);
    void Remove(DichVu dichVu);
}
