using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IChiSoTieuThuCommandRepository
{
    Task<ChiSoTieuThu?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ChiSoTieuThu?> GetByThangNamAsync(int canHoId, int dichVuId, int thang, int nam, CancellationToken cancellationToken = default);
    Task AddAsync(ChiSoTieuThu chiSoTieuThu, CancellationToken cancellationToken = default);
    void Update(ChiSoTieuThu chiSoTieuThu);
    void Remove(ChiSoTieuThu chiSoTieuThu);
}
