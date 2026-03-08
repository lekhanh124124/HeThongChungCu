using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Domain.Entities.ChungCu;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.EFRepositories;

public class CanHoEFRepository : ICanHoEFRepository
{
    private readonly EFDbContext _dbContext;

    public CanHoEFRepository(EFDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CanHo?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<CanHo>()
            .FirstOrDefaultAsync(c =>
                c.Id == id &&
                !c.IsDeleted,
                cancellationToken);
    }

    public async Task<bool> AnyAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<CanHo>()
            .AnyAsync(c =>
                c.Id == id &&
                !c.IsDeleted,
                cancellationToken);
    }

    public async Task<bool> MaCanHoExistsAsync(string maCanHo, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<CanHo>().AnyAsync(c => c.MaCanHo == maCanHo, cancellationToken);
    }

    public async Task<IReadOnlyList<CanHo>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<CanHo>()
            .Where(c =>
                ids.Contains(c.Id) &&
                !c.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(CanHo canHo, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<CanHo>().AddAsync(canHo, cancellationToken);
    }

    public void Update(CanHo canHo)
    {
        _dbContext.Set<CanHo>().Update(canHo);
    }

    public void Remove(CanHo canHo)
    {
        _dbContext.Set<CanHo>().Remove(canHo);
    }
}
