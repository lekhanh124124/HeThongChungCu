using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IDangKyDichVuCommandRepository
{
    Task<DangKyDichVu?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<DangKyDichVu?> GetActiveAsync(int canHoId, int dichVuId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DangKyDichVu>> GetByCanHoIdAsync(int canHoId, CancellationToken cancellationToken = default);
    Task AddAsync(DangKyDichVu dangKyDichVu, CancellationToken cancellationToken = default);
    void Update(DangKyDichVu dangKyDichVu);
}
