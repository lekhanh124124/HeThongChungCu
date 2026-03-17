using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.EFRepositories;

public class QuanHeCuTruEFRepository : IQuanHeCuTruEFRepository
{
    private readonly AppDbContext _dbContext;

    public QuanHeCuTruEFRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QuanHeCuTru?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<QuanHeCuTru>()
            .FirstOrDefaultAsync(q =>
                q.Id == id &&
                !q.IsDeleted,
                cancellationToken);
    }

    public async Task AddAsync(QuanHeCuTru quanHeCuTru, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<QuanHeCuTru>().AddAsync(quanHeCuTru, cancellationToken);
    }

    public void Update(QuanHeCuTru quanHeCuTru)
    {
        _dbContext.Set<QuanHeCuTru>().Update(quanHeCuTru);
    }

    public void Remove(QuanHeCuTru quanHeCuTru)
    {
        _dbContext.Set<QuanHeCuTru>().Remove(quanHeCuTru);
    }

    public async Task<IEnumerable<QuanHeCuTru>> GetByCanHoIdAsync(int canHoId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<QuanHeCuTru>()
            .Where(q => q.CanHoId == canHoId && !q.IsDeleted)
            .ToListAsync(cancellationToken);
    }
}
