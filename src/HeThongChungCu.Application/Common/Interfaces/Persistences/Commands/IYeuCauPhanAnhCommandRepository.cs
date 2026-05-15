using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IYeuCauPhanAnhCommandRepository
{
    Task<YeuCauPhanAnh?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<YeuCauPhanAnh?> GetByIdWithRepliesAsync(int id, CancellationToken cancellationToken = default);
    Task<YeuCauPhanAnh?> GetByIdWithFilesAsync(int id, CancellationToken cancellationToken = default);
    Task<YeuCauPhanAnh?> GetByIdWithAllAsync(int id, CancellationToken cancellationToken = default);
    Task<List<YeuCauPhanAnh>> GetOverdueNotNotifiedAsync(DateTimeOffset currentTime, CancellationToken cancellationToken = default);
    
    Task AddAsync(YeuCauPhanAnh phanAnh, CancellationToken cancellationToken = default);
    void Update(YeuCauPhanAnh phanAnh);
    void Delete(YeuCauPhanAnh phanAnh);
}
