using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class YeuCauCuTruCommandRepository : IYeuCauCuTruCommandRepository
{
    private readonly AppDbContext _dbContext;

    public YeuCauCuTruCommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<YeuCauCuTru?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<YeuCauCuTru>()
            .Include(x => x.YeuCauTaiLieuCuTrus)
                .ThenInclude(x => x.Files)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<YeuCauCuTru>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<YeuCauCuTru>()
            .Where(x => ids.Contains(x.Id))
            .Include(x => x.YeuCauTaiLieuCuTrus)
                .ThenInclude(x => x.Files)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<YeuCauCuTru>> GetByCanHoIdAsync(int canHoId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<YeuCauCuTru>()
            .Where(x => x.CanHoId == canHoId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<YeuCauCuTru>> GetByCanHoIdAndStatusesAsync(int canHoId, IEnumerable<TrangThaiYeuCau> statuses, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<YeuCauCuTru>()
            .Where(x => x.CanHoId == canHoId && statuses.Contains(x.TrangThaiId))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<YeuCauCuTru>> GetPendingRequestsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<YeuCauCuTru>()
            .Where(x => x.TrangThaiId == TrangThaiYeuCau.Pending)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<YeuCauCuTru, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<YeuCauCuTru>().AnyAsync(expression, cancellationToken);
    }

    public async Task AddAsync(YeuCauCuTru yeuCau, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<YeuCauCuTru>().AddAsync(yeuCau, cancellationToken);
    }

    public void Update(YeuCauCuTru yeuCau)
    {
        _dbContext.Set<YeuCauCuTru>().Update(yeuCau);
    }

    public void Delete(YeuCauCuTru yeuCau)
    {
        _dbContext.Set<YeuCauCuTru>().Remove(yeuCau);
    }

    public void DeleteRange(IEnumerable<YeuCauCuTru> yeuCaus)
    {
        _dbContext.Set<YeuCauCuTru>().RemoveRange(yeuCaus);
    }
}
