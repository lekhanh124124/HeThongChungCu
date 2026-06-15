using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class DoiTacCommandRepository : IDoiTacCommandRepository
{
    private readonly AppDbContext _dbContext;

    public DoiTacCommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DoiTac?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DoiTacs
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<DoiTac?> GetByIdWithHopDongsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DoiTacs
            .Include(t => t.HopDongs)
                .ThenInclude(h => h.TepHopDongs)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<DoiTac>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DoiTacs
            .Include(t => t.HopDongs)
            .Where(t => ids.Contains(t.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<HopDongDoiTac?> GetHopDongByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.HopDongDoiTacs
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

    public async Task AddAsync(DoiTac doiTac, CancellationToken cancellationToken = default)
    {
        await _dbContext.DoiTacs.AddAsync(doiTac, cancellationToken);
    }

    public void Update(DoiTac doiTac)
    {
        _dbContext.DoiTacs.Update(doiTac);
    }

    public void Remove(DoiTac doiTac)
    {
        _dbContext.DoiTacs.Remove(doiTac);
    }

    public void RemoveRange(IEnumerable<DoiTac> doiTacs)
    {
        _dbContext.DoiTacs.RemoveRange(doiTacs);
    }
}
