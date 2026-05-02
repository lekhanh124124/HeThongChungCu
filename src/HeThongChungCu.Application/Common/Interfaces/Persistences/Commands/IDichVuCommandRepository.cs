using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IDichVuCommandRepository
{
    Task<DichVu?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<DichVu>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<DichVu?> GetByIdWithKhungGiosAsync(int id, CancellationToken cancellationToken = default);
    Task<DichVu?> GetByIdWithBangGiasAsync(int id, CancellationToken cancellationToken = default);
    Task<DichVu?> GetByIdWithAllAsync(int id, CancellationToken cancellationToken = default);
    Task<List<DichVu>> GetByIdsWithAllAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<BangGia?> GetBangGiaByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<KhungGioDichVu?> GetKhungGioByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> MaDichVuExistsAsync(string maDichVu, CancellationToken cancellationToken = default);
    Task<List<DichVu>> GetByHopDongAsync(int hopDongId, CancellationToken cancellationToken = default);
    Task<List<DichVu>> GetActiveMandatoryServicesAsync(CancellationToken cancellationToken = default);
    Task<List<DichVu>> GetActivePeriodicServicesWithPriceListsAsync(CancellationToken cancellationToken = default);
    Task AddAsync(DichVu dichVu, CancellationToken cancellationToken = default);
    void Update(DichVu dichVu);
    void Remove(DichVu dichVu);
    void RemoveBangGia(BangGia bangGia);
    void RemoveKhungGio(KhungGioDichVu khungGio);
}
