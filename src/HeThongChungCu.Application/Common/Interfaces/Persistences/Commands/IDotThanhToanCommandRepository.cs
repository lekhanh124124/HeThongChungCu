using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IDotThanhToanCommandRepository
{
    Task AddAsync(DotThanhToan dot, CancellationToken cancellationToken = default);
    Task<DotThanhToan?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<DotThanhToan?> GetLatestOpenByKyAsync(KyThanhToan ky, CancellationToken cancellationToken = default);
}
