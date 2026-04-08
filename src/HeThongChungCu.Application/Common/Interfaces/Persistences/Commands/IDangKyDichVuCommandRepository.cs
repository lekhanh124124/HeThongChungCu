using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IDangKyDichVuCommandRepository
{
    Task AddAsync(DangKyDichVu dangKyDichVu, CancellationToken cancellationToken);
    Task<int> GetSumActiveQuantityByDichVuIdAsync(int dichVuId, CancellationToken cancellationToken);
    Task<int> GetSumActiveQuantityByKhungGioAsync(int dichVuId, TimeSpan gioBatDau, TimeSpan gioKetThuc, DateTime ngay, CancellationToken cancellationToken);
    Task<bool> IsCanHoRegisteredActiveAsync(int canHoId, int dichVuId, CancellationToken cancellationToken);
}
