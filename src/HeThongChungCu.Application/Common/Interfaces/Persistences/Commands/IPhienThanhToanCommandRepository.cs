using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IPhienThanhToanCommandRepository
{
    Task AddAsync(PhienThanhToan phien, CancellationToken cancellationToken = default);
    void Update(PhienThanhToan phien);
    Task<PhienThanhToan?> GetByMaThanhToanAsync(string maThanhToan, CancellationToken cancellationToken = default);
}
