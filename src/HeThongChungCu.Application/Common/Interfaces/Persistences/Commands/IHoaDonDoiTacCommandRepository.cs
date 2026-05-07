using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IHoaDonDoiTacCommandRepository
{
    Task AddAsync(HoaDonDoiTac hoaDon, CancellationToken cancellationToken = default);
    Task<HoaDonDoiTac?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByKyAsync(int hopDongId, int thang, int nam, CancellationToken cancellationToken = default);
    void Update(HoaDonDoiTac hoaDon);
    void Remove(HoaDonDoiTac hoaDon);
}
