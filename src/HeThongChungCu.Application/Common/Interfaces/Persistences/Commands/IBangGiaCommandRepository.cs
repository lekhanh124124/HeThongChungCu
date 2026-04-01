using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IBangGiaCommandRepository
{
    Task<BangGia?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BangGia>> GetByDichVuIdAsync(int dichVuId, CancellationToken cancellationToken = default);
    Task AddAsync(BangGia bangGia, CancellationToken cancellationToken = default);
    void Update(BangGia bangGia);
    void Remove(BangGia bangGia);
}
