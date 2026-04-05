using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class ToaNhaCommandRepository : IToaNhaCommandRepository
{
    private readonly AppDbContext _dbContext;

    public ToaNhaCommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ToaNha?> GetToaNhaByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ToaNha>()
            .Include(t => t.Tangs)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<ToaNha?> GetToaNhaByTangIdAsync(int tangId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ToaNha>()
            .Include(t => t.Tangs)
            .FirstOrDefaultAsync(t => t.Tangs.Any(tang => tang.Id == tangId), cancellationToken);
    }

    public async Task<bool> MaToaNhaExistsAsync(string maToaNha, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ToaNha>().AnyAsync(t => t.MaToaNha == maToaNha, cancellationToken);
    }

    public async Task<IReadOnlyList<ToaNha>> GetToaNhaByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ToaNha>()
            .Where(t => ids.Contains(t.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Tang>> GetTangByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Tang>()
            .Include(t => t.ToaNha)
            .Where(t => ids.Contains(t.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ToaNha toaNha, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<ToaNha>().AddAsync(toaNha, cancellationToken);
    }

    public void Update(ToaNha toaNha)
    {
        _dbContext.Set<ToaNha>().Update(toaNha);
    }

    public void Remove(object entity)
    {
        _dbContext.Remove(entity);
    }
}
