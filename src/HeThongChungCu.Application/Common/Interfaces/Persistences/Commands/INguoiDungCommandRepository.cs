using System.Linq.Expressions;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface INguoiDungCommandRepository
{
    Task<NguoiDung?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<NguoiDung?> GetByIdWithDocumentsAsync(int id, CancellationToken cancellationToken = default);
    Task<NguoiDung?> GetByCCCDAsync(string cccd, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<NguoiDung, bool>> expression, CancellationToken cancellationToken = default);

    Task<IEnumerable<NguoiDung>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task AddAsync(NguoiDung nguoiDung, CancellationToken cancellationToken = default);
    void Update(NguoiDung nguoiDung);
    void Delete(NguoiDung nguoiDung);
}
