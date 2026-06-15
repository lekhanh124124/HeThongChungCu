using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class YeuCauSuaChuaCommandRepository : IYeuCauSuaChuaCommandRepository
{
    private readonly AppDbContext _dbContext;

    public YeuCauSuaChuaCommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<YeuCauSuaChua?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.YeuCauSuaChuas
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<YeuCauSuaChua?> GetByIdWithFilesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.YeuCauSuaChuas
            .Include(x => x.TepYeuCauSuaChuas)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<YeuCauSuaChua?> GetByIdWithPersonnelAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.YeuCauSuaChuas
            .Include(x => x.NhanSuSuaChuas)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<YeuCauSuaChua>> GetByCanHoIdAndStatusesAsync(int canHoId, IEnumerable<HeThongChungCu.Domain.Enums.TrangThaiYeuCau> statuses, CancellationToken cancellationToken = default)
    {
        return await _dbContext.YeuCauSuaChuas
            .Where(x => x.CanHoId == canHoId && statuses.Contains(x.TrangThaiId))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<YeuCauSuaChua, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await _dbContext.YeuCauSuaChuas.AnyAsync(expression, cancellationToken);
    }

    public async Task AddAsync(YeuCauSuaChua ycsc, CancellationToken cancellationToken = default)
    {
        await _dbContext.YeuCauSuaChuas.AddAsync(ycsc, cancellationToken);
    }

    public void Update(YeuCauSuaChua ycsc)
    {
        _dbContext.YeuCauSuaChuas.Update(ycsc);
    }

    public void Delete(YeuCauSuaChua ycsc)
    {
        _dbContext.YeuCauSuaChuas.Remove(ycsc);
    }
}
