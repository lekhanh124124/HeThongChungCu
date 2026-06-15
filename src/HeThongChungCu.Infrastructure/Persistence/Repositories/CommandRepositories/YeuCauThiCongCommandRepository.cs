using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class YeuCauThiCongCommandRepository : IYeuCauThiCongCommandRepository
{
    private readonly AppDbContext _dbContext;

    public YeuCauThiCongCommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<YeuCauThiCong?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.YeuCauThiCongs
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<YeuCauThiCong?> GetByIdWithFilesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.YeuCauThiCongs
            .Include(x => x.TepYeuCauThiCongs)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<YeuCauThiCong?> GetByIdWithPersonnelAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.YeuCauThiCongs
            .Include(x => x.NhanSuThiCongs)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<YeuCauThiCong?> GetByIdWithAllAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.YeuCauThiCongs
            .Include(x => x.NhanSuThiCongs)
            .Include(x => x.TepYeuCauThiCongs)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<YeuCauThiCong>> GetByCanHoIdAndStatusesAsync(int canHoId, IEnumerable<HeThongChungCu.Domain.Enums.TrangThaiYeuCau> statuses, CancellationToken cancellationToken = default)
    {
        return await _dbContext.YeuCauThiCongs
            .Where(x => x.CanHoId == canHoId && statuses.Contains(x.TrangThaiId))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<YeuCauThiCong, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await _dbContext.YeuCauThiCongs.AnyAsync(expression, cancellationToken);
    }

    public async Task AddAsync(YeuCauThiCong yctc, CancellationToken cancellationToken = default)
    {
        await _dbContext.YeuCauThiCongs.AddAsync(yctc, cancellationToken);
    }

    public void Update(YeuCauThiCong yctc)
    {
        _dbContext.YeuCauThiCongs.Update(yctc);
    }

    public void Delete(YeuCauThiCong yctc)
    {
        _dbContext.YeuCauThiCongs.Remove(yctc);
    }
}
