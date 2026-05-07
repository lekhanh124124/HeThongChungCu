using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IGiaoDichThanhToanCommandRepository
{
    Task AddAsync(GiaoDichThanhToan giaoDich, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<GiaoDichThanhToan> giaoDichs, CancellationToken cancellationToken = default);

    Task<HashSet<int>> GetAllocatedChiTietHoaDonIdsAsync(IEnumerable<int> chiTietHoaDonIds, CancellationToken cancellationToken = default);
    Task<decimal> GetPaidAmountByHoaDonIdAsync(int hoaDonId, CancellationToken cancellationToken = default);
}
