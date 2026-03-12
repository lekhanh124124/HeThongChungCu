using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.EFRepositories;

public class ToaNhaEFRepository : IToaNhaEFRepository
{
    private readonly AppDbContext _dbContext;

    public ToaNhaEFRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ToaNha?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ToaNha>()
            .FirstOrDefaultAsync(t => 
                t.Id == id &&
                !t.IsDeleted, 
                cancellationToken);
    }

    public async Task<bool> AnyAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ToaNha>()
            .AnyAsync(t => 
                t.Id == id &&
                !t.IsDeleted, 
                cancellationToken);
    }

    public async Task<bool> MaToaNhaExistsAsync(string maToaNha, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ToaNha>().AnyAsync(t => t.MaToaNha == maToaNha, cancellationToken);
    }

    public async Task<IReadOnlyList<ToaNha>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ToaNha>()
            .Where(t => 
                ids.Contains(t.Id) &&
                !t.IsDeleted)
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

    public void Remove(ToaNha toaNha)
    {
        _dbContext.Set<ToaNha>().Remove(toaNha);
    }
}
