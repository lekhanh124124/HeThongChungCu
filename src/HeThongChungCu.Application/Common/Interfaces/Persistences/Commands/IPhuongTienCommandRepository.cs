using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IPhuongTienCommandRepository
{
    Task<PhuongTien> AddAsync(PhuongTien phuongTien, CancellationToken cancellationToken = default);
    Task<PhuongTien?> GetPhuongTienByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<PhuongTien>> GetPhuongTiensByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<List<PhuongTien>> GetPhuongTiensByCanHoIdAsync(int canHoId, CancellationToken cancellationToken = default);
    Task<List<PhuongTien>> GetPhuongTiensByTheIdsAsync(IEnumerable<int> theIds, CancellationToken cancellationToken = default);
    Task<bool> BienSoExistsAsync(string bienSo, CancellationToken cancellationToken = default);
    Task<bool> MaTheExistsAsync(string maThe, CancellationToken cancellationToken = default);
    Task<int> GetMaxThePhuongTienIdAsync(CancellationToken cancellationToken = default);
    Task<List<PhuongTien>> GetActiveByCanHoIdsAsync(IEnumerable<int> canHoIds, CancellationToken cancellationToken = default);
    void Update(PhuongTien phuongTien);
    void RemoveRange(IEnumerable<PhuongTien> phuongTiens);
}
