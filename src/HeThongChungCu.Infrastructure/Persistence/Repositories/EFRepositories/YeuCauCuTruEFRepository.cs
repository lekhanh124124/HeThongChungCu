using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.EFRepositories;

public class YeuCauCuTruEFRepository : IYeuCauCuTruEFRepository
{
    private readonly AppDbContext _dbContext;

    public YeuCauCuTruEFRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<YeuCauCuTru?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<YeuCauCuTru>()
            .Include(x => x.CanHo)
            .Include(x => x.QuanHeCuTru)
            .Include(x => x.Documents)
                .ThenInclude(x => x.Files)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<YeuCauCuTru>> GetByCanHoIdAsync(int canHoId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<YeuCauCuTru>()
            .Where(x => x.CanHoId == canHoId)
            .OrderByDescending(x => x.CreatedAt)
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
}
