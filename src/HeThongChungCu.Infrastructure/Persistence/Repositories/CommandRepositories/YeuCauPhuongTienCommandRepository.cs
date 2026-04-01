using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class YeuCauPhuongTienCommandRepository : IYeuCauPhuongTienCommandRepository
{
    private readonly AppDbContext _dbContext;

    public YeuCauPhuongTienCommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<YeuCauPhuongTien?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<YeuCauPhuongTien>()
            .Include(x => x.YeuCauHinhAnhPhuongTiens)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<YeuCauPhuongTien>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<YeuCauPhuongTien>()
            .Where(x => ids.Contains(x.Id))
            .Include(x => x.YeuCauHinhAnhPhuongTiens)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<YeuCauPhuongTien>> GetByCanHoIdAsync(int canHoId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<YeuCauPhuongTien>()
            .Where(x => x.CanHoId == canHoId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<YeuCauPhuongTien>> GetByCanHoIdAndStatusesAsync(int canHoId, IEnumerable<TrangThaiYeuCau> statuses, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<YeuCauPhuongTien>()
            .Where(x => x.CanHoId == canHoId && statuses.Contains(x.TrangThaiId))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<YeuCauPhuongTien>> GetPendingRequestsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<YeuCauPhuongTien>()
            .Where(x => x.TrangThaiId == TrangThaiYeuCau.Pending)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<YeuCauPhuongTien, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<YeuCauPhuongTien>().AnyAsync(expression, cancellationToken);
    }

    public async Task AddAsync(YeuCauPhuongTien yeuCau, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<YeuCauPhuongTien>().AddAsync(yeuCau, cancellationToken);
    }

    public void Update(YeuCauPhuongTien yeuCau)
    {
        _dbContext.Set<YeuCauPhuongTien>().Update(yeuCau);
    }

    public void Delete(YeuCauPhuongTien yeuCau)
    {
        _dbContext.Set<YeuCauPhuongTien>().Remove(yeuCau);
    }

    public void DeleteRange(IEnumerable<YeuCauPhuongTien> yeuCaus)
    {
        _dbContext.Set<YeuCauPhuongTien>().RemoveRange(yeuCaus);
    }
}
