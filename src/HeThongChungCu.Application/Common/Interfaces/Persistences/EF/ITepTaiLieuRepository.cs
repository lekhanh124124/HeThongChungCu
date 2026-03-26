using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.EF;

public interface ITepTaiLieuRepository
{
    Task<TepTaiLieu?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TepTaiLieu>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<IEnumerable<TepTaiLieu>> GetUnusedFilesAsync(DateTime before, CancellationToken cancellationToken = default);
    Task AddAsync(TepTaiLieu file, CancellationToken cancellationToken = default);
    void Update(TepTaiLieu file);
    void Delete(TepTaiLieu file);
    void DeleteRange(IEnumerable<TepTaiLieu> files);
}
