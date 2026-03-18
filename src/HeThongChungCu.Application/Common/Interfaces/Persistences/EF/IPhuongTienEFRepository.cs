using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.EF;

public interface IPhuongTienEFRepository
{
    Task AddAsync(PhuongTien phuongTien, CancellationToken cancellationToken = default);
    Task<PhuongTien?> GetPhuongTienByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<PhuongTien>> GetPhuongTiensByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<List<PhuongTien>> GetPhuongTiensByCanHoIdAsync(int canHoId, CancellationToken cancellationToken = default);
    Task<List<PhuongTien>> GetPhuongTiensByTheIdsAsync(IEnumerable<int> theIds, CancellationToken cancellationToken = default);
    Task<bool> BienSoExistsAsync(string bienSo, CancellationToken cancellationToken = default);
    Task<bool> MaTheExistsAsync(string maThe, CancellationToken cancellationToken = default);
    void Update(PhuongTien phuongTien);
    void RemoveRange(IEnumerable<PhuongTien> phuongTiens);
}
