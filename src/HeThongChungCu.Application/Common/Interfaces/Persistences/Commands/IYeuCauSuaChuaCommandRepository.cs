using HeThongChungCu.Domain.Entities;
using System.Linq.Expressions;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface IYeuCauSuaChuaCommandRepository
{
    Task<YeuCauSuaChua?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<YeuCauSuaChua?> GetByIdWithFilesAsync(int id, CancellationToken cancellationToken = default);
    Task<YeuCauSuaChua?> GetByIdWithPersonnelAsync(int id, CancellationToken cancellationToken = default);
    Task<List<YeuCauSuaChua>> GetByCanHoIdAndStatusesAsync(int canHoId, IEnumerable<HeThongChungCu.Domain.Enums.TrangThaiYeuCau> statuses, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<YeuCauSuaChua, bool>> expression, CancellationToken cancellationToken = default);

    Task AddAsync(YeuCauSuaChua ycsc, CancellationToken cancellationToken = default);
    void Update(YeuCauSuaChua ycsc);
    void Delete(YeuCauSuaChua ycsc);
}
