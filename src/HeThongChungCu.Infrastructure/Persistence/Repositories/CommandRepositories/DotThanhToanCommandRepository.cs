using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.CommandRepositories;

public class DotThanhToanCommandRepository : IDotThanhToanCommandRepository
{
    private readonly AppDbContext _dbContext;

    public DotThanhToanCommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(DotThanhToan dot, CancellationToken cancellationToken = default)
    {
        await _dbContext.DotThanhToan.AddAsync(dot, cancellationToken);
    }

    public async Task<DotThanhToan?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DotThanhToan.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<DotThanhToan>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DotThanhToan.Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);
    }

    public async Task<DotThanhToan?> GetLatestOpenByKyAsync(KyThanhToan ky, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DotThanhToan
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync(x => 
                x.KyThanhToan.Thang == ky.Thang && 
                x.KyThanhToan.Nam == ky.Nam && 
                (x.TrangThaiDotThanhToanId == TrangThaiDotThanhToan.TaoMoi || 
                 x.TrangThaiDotThanhToanId == TrangThaiDotThanhToan.DaPhatHanh ||
                 x.TrangThaiDotThanhToanId == TrangThaiDotThanhToan.DaDuyet ||
                 x.TrangThaiDotThanhToanId == TrangThaiDotThanhToan.DaLapDuThao), 
                cancellationToken);
    }

    public async Task<bool> ExistsByKyThanhToanExcludeIdAsync(KyThanhToan ky, int excludeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DotThanhToan.AnyAsync(x => 
            x.KyThanhToan.Thang == ky.Thang && 
            x.KyThanhToan.Nam == ky.Nam && 
            x.Id != excludeId, 
            cancellationToken);
    }

    public void Delete(DotThanhToan dot)
    {
        _dbContext.DotThanhToan.Remove(dot);
    }

    public void DeleteRange(IEnumerable<DotThanhToan> dots)
    {
        _dbContext.DotThanhToan.RemoveRange(dots);
    }
}
