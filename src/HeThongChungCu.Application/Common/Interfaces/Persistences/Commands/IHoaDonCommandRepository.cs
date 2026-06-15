using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IHoaDonCommandRepository
{
    Task AddAsync(HoaDon hoaDon, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<HoaDon> hoaDons, CancellationToken cancellationToken = default);
    Task<HoaDon?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<HoaDon>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<List<HoaDon>> GetByDotThanhToanAsync(int dotId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByKyThanhToanAsync(int canHoId, KyThanhToan ky, CancellationToken cancellationToken = default);
    Task<HashSet<int>> GetExistingCanHoIdsByKyAsync(KyThanhToan ky, CancellationToken cancellationToken = default);
    Task<bool> AnyByDotThanhToanAsync(int dotId, CancellationToken cancellationToken = default);
    Task<bool> HasUnpaidInvoicesAsync(int canHoId, CancellationToken cancellationToken = default);
    Task<bool> AnyInvoicesByCanHoAsync(int canHoId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy tất cả hóa đơn quá hạn của các căn hộ chỉ định, chưa tính lãi trong kỳ hiện tại.
    /// Điều kiện: ChuaThanhToan/QuaHan, NgayHanThanhToan < today, NgayTinhLaiCuoi IS NULL hoặc < dotStartDate.
    /// </summary>
    Task<ILookup<int, HoaDon>> GetOverdueByCanHoIdsAsync(IEnumerable<int> canHoIds, DateTimeOffset dotStartDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách các hóa đơn chưa thanh toán hoặc thanh toán một phần đã quá hạn thanh toán.
    /// </summary>
    Task<List<HoaDon>> GetPendingPastDueInvoicesAsync(DateTimeOffset referenceDate, CancellationToken cancellationToken = default);

    void Update(HoaDon hoaDon);
}
