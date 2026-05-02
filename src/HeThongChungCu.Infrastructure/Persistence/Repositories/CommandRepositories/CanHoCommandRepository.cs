using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class CanHoCommandRepository : ICanHoCommandRepository
{
    private readonly AppDbContext _dbContext;

    public CanHoCommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CanHo?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CanHos
            .FirstOrDefaultAsync(c =>
                c.Id == id,
                cancellationToken);
    }

    public async Task<bool> AnyAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CanHos
            .AnyAsync(c =>
                c.Id == id,
                cancellationToken);
    }

    public async Task<bool> MaCanHoExistsAsync(string maCanHo, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CanHos.AnyAsync(c => c.MaCanHo == maCanHo, cancellationToken);
    }

    public async Task<IReadOnlyList<CanHo>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CanHos
            .Where(c => ids.Contains(c.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CanHo>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.CanHos
            .Where(x => x.TinhTrangCanHoId != TrangThaiCanHo.ChuaBanGiao)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(CanHo canHo, CancellationToken cancellationToken = default)
    {
        await _dbContext.CanHos.AddAsync(canHo, cancellationToken);
    }

    public void Update(CanHo canHo)
    {
        _dbContext.CanHos.Update(canHo);
    }

    public void Remove(CanHo canHo)
    {
        _dbContext.CanHos.Remove(canHo);
    }
}
