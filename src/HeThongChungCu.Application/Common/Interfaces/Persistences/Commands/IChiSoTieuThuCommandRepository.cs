using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IChiSoTieuThuCommandRepository
{
    Task AddAsync(ChiSoTieuThu chiSo, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<ChiSoTieuThu> chiSos, CancellationToken cancellationToken = default);
    Task<ChiSoTieuThu?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<ChiSoTieuThu>> GetLockedUnbilledByPeriodAsync(KyThanhToan ky, CancellationToken cancellationToken = default);
    Task<List<ChiSoTieuThu>> GetLockedUnbilledByCanHoAsync(int canHoId, KyThanhToan ky, CancellationToken cancellationToken = default);
    Task<ChiSoTieuThu?> GetLatestByCanHoAndDichVuAsync(int canHoId, int dichVuId, CancellationToken cancellationToken = default);
    Task<List<ChiSoTieuThu>> GetByPeriodAsync(int thang, int nam, CancellationToken cancellationToken = default);
    Task<List<ChiSoTieuThu>> GetByMaTraCuusAsync(IEnumerable<string> codes, CancellationToken cancellationToken = default);
    void Update(ChiSoTieuThu chiSo);
}
